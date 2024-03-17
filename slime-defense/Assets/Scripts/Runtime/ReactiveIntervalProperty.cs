using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Game
{
    public class ReactiveIntervalProperty<T> : ReactiveProperty<T>
    {
        private float intervalTime;

        public float Interval { get; set; }

        public override T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (intervalTime > 0) return;
                if (!EqualityComparer.Equals(this.value, value))
                {
                    intervalTime = Interval;
                    SetValue(value);
                    if (isDisposed)
                        return;

                    RaiseOnNext(ref value);
                }
            }
        }

        public ReactiveIntervalProperty(Component component) : base()
        {
            component
                .UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (intervalTime > 0)
                        intervalTime -= Time.deltaTime;
                });
        }
    }
}