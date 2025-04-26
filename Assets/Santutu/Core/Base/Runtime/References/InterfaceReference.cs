#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Santutu.Core.Base.Runtime.References
{
    [Serializable]
    public class InterfaceReference<TInterface, TObject> : ISerializationCallbackReceiver where TObject : Object where TInterface : class
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [HideIf(nameof(HideUnderlyingObject))]
#endif
        protected TObject underlyingObjectValue;

        [SerializeReference]
#if ODIN_INSPECTOR
        [HideIf(nameof(HideUnderlyingValue))]
#endif
        protected TInterface underlyingValue;

        public bool IsObjectValue => underlyingObjectValue != null;
        public bool IsReferenceValue => !IsObjectValue;

        public bool HasValue => underlyingObjectValue != null || underlyingValue != null;


        protected bool HideUnderlyingObject => HasValue && IsReferenceValue;
        protected bool HideUnderlyingValue => HasValue && IsObjectValue;

        public TInterface Value
        {
            get
            {
                if (underlyingObjectValue == null && underlyingValue == null)
                {
                    return null;
                }

                if (IsObjectValue)
                {
                    return underlyingObjectValue as TInterface;
                }

                if (IsReferenceValue)
                {
                    return underlyingValue;
                }


                throw new InvalidOperationException($"{underlyingObjectValue} needs to implement interface {nameof(TInterface)}.");
            }
            set
            {
                underlyingObjectValue = null;
                underlyingValue = null;

                if (value is TObject obj)
                {
                    underlyingObjectValue = obj;
                    return;
                }

                underlyingValue = value;
            }
        }

        public TObject UnderlyingObjectValue
        {
            get => underlyingObjectValue;
            set
            {
                underlyingValue = null;
                underlyingObjectValue = value;
            }
        }

        public TInterface UnderlyingReferenceValue
        {
            get => underlyingValue;
            set
            {
                underlyingObjectValue = null;
                underlyingValue = value;
            }
        }

        public InterfaceReference()
        {
        }

        public InterfaceReference(TObject target)
        {
            underlyingObjectValue = target;
        }

        public InterfaceReference(TInterface @interface)
        {
            underlyingObjectValue = @interface as TObject;
        }

        public static implicit operator TInterface(InterfaceReference<TInterface, TObject> obj)
        {
            return obj.Value;
        }

        public void OnBeforeSerialize()
        {
            if (underlyingObjectValue == null)
            {
                return;
            }

            if (underlyingObjectValue is not TInterface)
            {
                Debug.LogWarning($"{underlyingObjectValue.GetType()}, {typeof(TInterface)}", underlyingObjectValue);
                underlyingObjectValue = null;
            }
        }

        public void OnAfterDeserialize()
        {
            // if (underlyingObjectValue == null)
            // {
            //     return;
            // }
            //
            // if (underlyingObjectValue != null && underlyingObjectValue is not TInterface)
            // {
            //     Debug.LogWarning($"{underlyingObjectValue.GetType()}, {typeof(TInterface)}", underlyingObjectValue);
            //     underlyingObjectValue = null;
            // }
        }
    }

    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class
    {
        public InterfaceReference()
        {
        }

        public InterfaceReference(TInterface @interface) : base(@interface)
        {
        }

        public static implicit operator TInterface(InterfaceReference<TInterface> obj)
        {
            return obj.Value;
        }
    }
}