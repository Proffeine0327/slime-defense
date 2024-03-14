using System.Collections;
using Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.DeckSettingScene
{
    public partial class Slime : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //service
        private InputManager inputManager => ServiceProvider.Get<InputManager>();
        private DeckSettingManager deckSettingManager => ServiceProvider.Get<DeckSettingManager>();

        private int index;
        private string key;
        private bool isDragging;
        private bool isInDeck;
        private Vector3 targetPosition;
        private Vector3 startDraggingPosition;

        public string Key => key;

        public void MoveToDeck(int index, Vector3 pos)
        {
            isInDeck = true;
            this.index = index;
            transform.position = pos;
        }

        public void RemoveFromDeck(Vector3 pos)
        {
            isInDeck = false;
            index = -1;
            transform.position = pos;
        }

        private IEnumerator MoveRoutine()
        {
            while (true)
            {
                targetPosition = GetRandomPos();

                while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
                {
                    yield return null;
                    if (isDragging || isInDeck)
                    {
                        transform.forward = Vector3.forward;
                        continue;
                    }

                    transform.LookAt(targetPosition);
                    transform.position = transform.position + transform.forward * Time.deltaTime * Mathf.Abs(Mathf.Sin(Time.time * 3));
                }

                yield return new WaitForSeconds(Random.Range(1f, 5f));
            }
        }

        private void Start()
        {
            if (!isInDeck) transform.position = GetRandomPos();
            StartCoroutine(MoveRoutine());
        }

        private Vector3 GetRandomPos()
        {
            return new(Random.Range(-3.75f, 3.75f), 0.5f, Random.Range(-1.75f, -5.25f));
        }

        private bool IsInPos(Vector3 pos)
        {
            return pos.z < -1.75f && pos.z > -5.25f && pos.x > -3.75f && pos.x < 3.75f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            deckSettingManager.Select(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            deckSettingManager.Select(null);
            isDragging = true;
            startDraggingPosition = transform.position;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            var plane = new Plane(Vector3.up, new Vector3(0, 0.5f, 0));
            if (plane.Raycast(inputManager.TouchRay, out var enter))
                transform.position = inputManager.TouchRay.GetPoint(enter);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;

            if (Physics.Raycast(inputManager.TouchRay, out var hit, Mathf.Infinity, ~LayerMask.GetMask("Slime")) && hit.collider.TryGetComponent<Slot>(out var comp))
            {
                if (!isInDeck)
                    deckSettingManager.SetDeck(comp.Index, key);
                else
                    deckSettingManager.ChangeDeck(index, comp.Index);
            }
            else
            {
                if (isInDeck)
                {
                    transform.position = startDraggingPosition;
                    return;
                }

                var plane = new Plane(Vector3.up, new Vector3(0, 0.5f, 0));
                if (plane.Raycast(inputManager.TouchRay, out var enter) && IsInPos(inputManager.TouchRay.GetPoint(enter)))
                {
                    transform.position = inputManager.TouchRay.GetPoint(enter);
                    targetPosition = transform.position;
                }
                else
                    transform.position = startDraggingPosition;
            }
        }
    }
}