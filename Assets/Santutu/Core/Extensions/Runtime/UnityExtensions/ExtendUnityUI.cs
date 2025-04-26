using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public interface ITwoBindableField<T>
    {
        public Subject<T> OnValueChanged { get; }
        public T Value { get; set; }

        public Component Disposable { get; }
    }

    public static class ExtendUnityUI
    {
        public static void BindTwoWays<T>(
            this ITwoBindableField<T> input, ReactiveProperty<T> target
        )
        {
            target.Subscribe(value => input.Value = value)
                .AddTo(input.Disposable);

            input.OnValueChanged
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = value)
                .AddTo(input.Disposable);
        }


        public static void BindTwoWays<TFieldValue, TModelValue>(
            this ITwoBindableField<TFieldValue> input, ReactiveProperty<TModelValue> target,
            Func<TModelValue, TFieldValue> from, Func<TFieldValue, TModelValue> to
        )
        {
            target.Subscribe(value => input.Value = from(value))
                .AddTo(input.Disposable);

            input.OnValueChanged
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = to(value))
                .AddTo(input.Disposable);
        }


        public static void BindTwoWays(
            this TMP_InputField input, ReactiveProperty<int> target
        )
        {
            input.BindTwoWays(target, model => model.ToString(), (text) => int.Parse(text));
        }

        public static void BindTwoWays(
            this TMP_InputField input, ReactiveProperty<string> target
        )
        {
            input.BindTwoWays(target, model => model, (text) => text);
        }


        public static IDisposable BindTwoWays(
            this TMP_Dropdown dropdown, ReactiveProperty<string> target
        )
        {
            return dropdown.BindTwoWays<string>(
                target,
                dropdown.GetValueByOptionText,
                (value) => dropdown.GetOptionByIndex(value).text
            );
        }
        
        public static IDisposable BindTwoWays(
            this TMP_Dropdown dropdown, ReactiveProperty<int> target
        )
        {
            var disposable = new CompositeDisposable();
            
            target.Subscribe(
                    value =>
                    {
                        dropdown.value = value;
                        dropdown.RefreshShownValue();
                    }
                )
                .AddTo(disposable);
            
            
            dropdown.onValueChanged.AsObservable()
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = value)
                .AddTo(disposable);

            return disposable;
        }


        public static IDisposable BindTwoWays<T>(
            this TMP_Dropdown dropdown, ReactiveProperty<T> target, Func<T, int> from, Func<int, T> to
        )
        {
            var disposable = new CompositeDisposable();
            
            target.Subscribe(
                    value =>
                    {
                        dropdown.value = from(value);
                        dropdown.RefreshShownValue();
                    }
                )
                .AddTo(disposable);

            dropdown.onValueChanged.AsObservable()
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = to(value))
                .AddTo(disposable);

            return disposable;
        }

        public static void BindTwoWays<T>(
            this TMP_InputField input, ReactiveProperty<T> target, Func<T, string> from, Func<string, T> to
        )
        {
            target.Subscribe(value => input.text = from(value))
                .AddTo(input);

            input.onValueChanged.AsObservable()
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = to(value))
                .AddTo(input);
        }

        public static IDisposable BindTwoWays(this Slider slider, ReactiveProperty<float> target)
        {
            var disposable = new CompositeDisposable().AddTo(slider);
            target.Subscribe(value => slider.value = value)
                .AddTo(disposable);


            slider.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = value)
                .AddTo(disposable);


            return disposable;
        }

        public static IDisposable BindTwoWays<T>(this ReactiveProperty<T> ui, ReactiveProperty<T> data)
        {
            var disposable = new CompositeDisposable();

            data.Subscribe((value) => ui.Value = value)
                .AddTo(disposable); 

            ui.Subscribe(value => data.Value = value)
                .AddTo(disposable);


            return disposable;
        }


        public static ReactiveProperty<float> BindTwoWays(this ReactiveProperty<float> target, Slider slider)
        {
            target.Subscribe(value => slider.value = value)
                .AddTo(slider);

            slider.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe((value) => target.Value = value)
                .AddTo(slider);

            return target;
        }
    }
}