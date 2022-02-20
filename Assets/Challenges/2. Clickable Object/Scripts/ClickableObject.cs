using System;
using UnityEngine;

namespace Challenges._2._Clickable_Object.Scripts
{
    public class InvalidInteractionMethodException : Exception
    {
        private const string MessageWithMethodArgument =
            "Attempted to register to an invalid method of clickable interaction. The ClickableObject '{0}' does not allow interaction of type {1}";
        public InvalidInteractionMethodException(string gameObjectName, ClickableObject.InteractionMethod interactionMethod) : base(string.Format(MessageWithMethodArgument,gameObjectName,interactionMethod))
        {
        }
    }
    [RequireComponent(typeof(Collider))]
    public class ClickableObject : MonoBehaviour, IClickableObject
    {
      
        // Do not remove the provided 3 options, you can add more if you like
        [Flags]
        public enum InteractionMethod
        {
            Tap=2,
            DoubleTap=4,
            TapAndHold=8
        }
        
        
        /// <summary>
        /// Dont edit
        /// </summary>
        [SerializeField]
        private InteractionMethod allowedInteractionMethods;


        event OnClickableClicked OnClick;
        OnClickableClicked clickCallback;
        private float objectSelectedTime;

        /// <summary>
        /// Checks if the given interaction method is valid for this clickable object.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool IsInteractionMethodValid(InteractionMethod method)
        {
            return allowedInteractionMethods.HasFlag(method);
        }


        /// <summary>
        /// Updates the interaction method of the clickable object. Can contain more than one value due to bitflags
        /// </summary>
        public void SetInteractionMethod(InteractionMethod method)
        {
            allowedInteractionMethods = method;
        }
        
        
        /// <summary>
        /// Will invoke the given callback when the clickable object is interacted with alongside the method of interaction
        /// </summary>
        /// <param name="callback">Function to invoke</param>
        public void RegisterToClickable(OnClickableClicked callback)
        {
            if(clickCallback == null) clickCallback = callback;
            OnClick += callback;
        }

        /// <summary>
        /// Will unregister a previously provided callback
        /// </summary>
        /// <param name="callback">Function previously given</param>
        public void UnregisterFromClickable(OnClickableClicked callback)
        {
            OnClick -= clickCallback;
        }

        /// <summary>
        /// Will invoke the given callback when the clickable object is tapped. 
        /// </summary>
        /// <param name="onTapCallback"></param>
        /// <exception cref="InvalidInteractionMethodException">If tapping is not allowed for this clickable</exception>
        public void RegisterToClickableTap(OnClickableClickedUnspecified onTapCallback) 
        {
            //OnClick = onTapCallback;
        }
        
        /// <summary>
        /// Will invoke the given callback when the clickable object is tapped. 
        /// </summary>
        /// <param name="onTapCallback"></param>
        /// <exception cref="InvalidInteractionMethodException">If double tapping is not allowed for this clickable</exception>
        public void RegisterToClickableDoubleTap(OnClickableClickedUnspecified onTapCallback) 
        {
            //OnClick = onTapCallback;
        }
        //
        private void OnTap(){
            OnClick(this, InteractionMethod.Tap);
            UnregisterFromClickable(OnClick);
        }
        private void OnTapnHold(){
            OnClick(this, InteractionMethod.TapAndHold);
            UnregisterFromClickable(OnClick);
        }
        private void OnDoubleTap(){
            OnClick(this, InteractionMethod.DoubleTap);
            UnregisterFromClickable(OnClick);
        }


        /// <summary>
        /// OnMouseDown is called when the user has pressed the mouse button while
        /// over the GUIElement or Collider.
        /// </summary>
        void OnMouseDown()
        {
            RegisterToClickable(clickCallback);
            if(objectSelectedTime == 0){
                objectSelectedTime = Time.time;
                if(IsInteractionMethodValid(ClickableObject.InteractionMethod.Tap))
                {
                    OnTap();
                }
            }
            else{
                CancelInvoke();
                if(IsInteractionMethodValid(ClickableObject.InteractionMethod.DoubleTap))
                {
                    OnDoubleTap();
                }
            }
        }

        /// <summary>
        /// OnMouseDrag is called when the user has clicked on a GUIElement or Collider
        /// and is still holding down the mouse.
        /// </summary>
        void OnMouseDrag()
        {
            if(OnClick == null) return;
            if(objectSelectedTime + 0.5f < Time.time) 
            {
                if(IsInteractionMethodValid(ClickableObject.InteractionMethod.TapAndHold))
                {
                    OnTapnHold();
                }
            }
        }
        /// <summary>
        /// OnMouseUp is called when the user has released the mouse button.
        /// </summary>
        void OnMouseUp()
        {
            Invoke("ResetClickTime",0.2f);
        }

        void ResetClickTime()
        {
            objectSelectedTime = 0;
        }
        
    }
}
