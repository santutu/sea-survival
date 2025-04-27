using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace Santutu.Modules.MultiScenes.Runtime
{
    public class ObservableProgress<T> : Observable<T>, IProgress<T>, IDisposable
    {
        public T Value;
        private readonly Subject<T> _onProgressChanged = new();

        public void Report(T value)
        {
            Value = value;
            _onProgressChanged.OnNext(value);
        }


        protected override IDisposable SubscribeCore(Observer<T> observer)
        {
            return ObservableSubscribeExtensions.Subscribe(_onProgressChanged, observer.OnNext);
        }

        public IDisposable ListenFrom(
            List<ObservableProgress<float>> progresses
        )
        {
            return progresses.Merge()
                             .Subscribe(_ => {
                                      var average = progresses.Average(p => p.Value);
                                      ((IProgress<float>)this).Report(average);
                                  }
                              );
        }

        public void Dispose()
        {
            _onProgressChanged?.Dispose();
        }
    }
}