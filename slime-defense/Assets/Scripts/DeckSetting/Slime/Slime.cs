using System.Collections;
using Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.DeckSettingScene
{
    public partial class Slime : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        //service
        private InputManager inputManager => ServiceProvider.Get<InputManager>();
        private DeckSettingManager deckSettingManager => ServiceProvider.Get<DeckSettingManager>();

        private string key;
        private bool isDragging;
        private bool isInDeck;
        private Vector3 targetPosition;
        private Vector3 startDraggingPosition;

        public void MoveToDeck(Vector3 pos)
        {
            isInDeck = true;
            transform.position = pos;
        }

        public void RemoveFromDeck(Vector3 pos)
        {
            isInDeck = false;
            transform.position = pos;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(isInDeck) return;

            isDragging = true;
            startDraggingPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!isDragging) return;
            var plane = new Plane(Vector3.up, new Vector3(0, 0.5f, 0));
            if (plane.Raycast(inputManager.TouchRay, out var enter))
                transform.position = inputManager.TouchRay.GetPoint(enter);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!isDragging) return;

            isDragging = false;
            if (Physics.Raycast(inputManager.TouchRay, out var hit, Mathf.Infinity, ~LayerMask.GetMask("Slime")) && hit.collider.TryGetComponent<Slot>(out var comp))
            {
                deckSettingManager.SetDeck(comp.Index, key);
            }
            else
            {
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
            if(!isInDeck) transform.position = GetRandomPos();
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
    }
}