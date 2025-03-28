using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DominoGames.Core.EventSystem;

namespace DominoGames.UI.Input{
    public class MobileJoystick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
    #region Informations
    /*
        모바일 조이스틱 구현단입니다
    */
    #endregion

    //----------------- Public -----------------
    #region Public Methods
        bool isBeginDragged = false;
        // 조이스틱 처리
        public void OnBeginDrag(PointerEventData eventData){
            if (!canInput)
            {
                return;
            }

            isBeginDragged = true;

            joystickOuter.transform.localPosition = eventData.position - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            joystickOuter.color = joystickFocusColor;
            joystickInner.color = joystickFocusColor;
            this._joystickEventData = eventData;
            DominoEventSystem.Pub(EEventTypes.Joystick_OnBegin, eventData);
        }
        public void OnEndDrag(PointerEventData eventData){
            if (!canInput)
            {
                return;
            }

            if (!isBeginDragged)
            {
                return;
            }

            isBeginDragged = false;

            joystickOuter.color = joystickNotfocusColor;
            joystickInner.color = joystickNotfocusColor;
            this._joystickEventData = null;

            joystickOuter.transform.localPosition = joystickOriginPosition;
            joystickInner.transform.localPosition = joystickOriginPosition;

            joystickInner.transform.position = joystickOuter.transform.position;
            DominoEventSystem.Pub(EEventTypes.Joystick_OnEnd, eventData);
        }
        public void OnDrag(PointerEventData eventData){
            if (!canInput)
            {
                return;
            }

            if (!isBeginDragged)
            {
                return;
            }

            Vector3 _touchPos = this._joystickEventData.position - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector3 _currentPos = joystickInner.transform.localPosition;
            _currentPos.x = _touchPos.x;
            _currentPos.y = _touchPos.y;

            joystickInner.transform.localPosition = _currentPos;

            joystickInner.transform.localPosition = Vector3.ClampMagnitude(joystickInner.transform.localPosition - joystickOuter.transform.localPosition, joystickRadius) + joystickOuter.transform.localPosition;
        }

        public Vector2 GetJoystickDirectionValue(){
            return (joystickInner.transform.position - joystickOuter.transform.position).normalized;
        }
    #endregion

    #region Public Properties
        public Image joystickOuter, joystickInner;
        public Color joystickFocusColor, joystickNotfocusColor;
        public float joystickRadius;
        public Vector2 joystickOriginPosition;
        public static bool canInput = true;
    #endregion

    //----------------- Unity -----------------
    #region Unity Methods
        void Awake(){
            this._rootCanvas = transform.root.GetComponent<Canvas>();
            joystickOuter.color = joystickNotfocusColor;
            joystickInner.color = joystickNotfocusColor;
        }

        void Update()
        {
            Vector2 direction = GetJoystickDirectionValue();
            if(direction != Vector2.zero && canInput && isBeginDragged)
            {
                DominoEventSystem.Pub(EEventTypes.Joystick_OnUpdate, direction);
            }
        }
    #endregion

    //----------------- Private -----------------
    #region Private Methods

    #endregion

    #region Private Properties
        private PointerEventData _joystickEventData = null;
        private Canvas _rootCanvas = null;
        private Vector2 _screenHalfSize = new Vector2(){
            x = Screen.width * 0.5f,
            y = Screen.height * 0.5f
        };
        #endregion
    }
}