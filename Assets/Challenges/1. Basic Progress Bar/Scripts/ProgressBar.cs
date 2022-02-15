using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Challenges._1._Basic_Progress_Bar.Scripts
{
    /// <summary>
    /// Edit this script for the ProgressBar challenge.
    /// </summary>
    public class ProgressBar : MonoBehaviour, IProgressBar
    {
        /// <summary>
        /// You can add more options
        /// </summary>
        private enum ProgressSnapOptions
        {
            SnapToLowerValue,
            SnapToHigherValue,
            DontSnap
        }
        
        /// <summary>
        /// You can add more options
        /// </summary>
        private enum TextPosition
        {
            BarCenter,
            Progress,
            NoText
        }
        [SerializeField] private Image fillBar;
        [SerializeField] private TMPro.TextMeshProUGUI percentageText;
        private float desiredValue;
        
        /// <summary>
        /// These settings below must function
        /// </summary>
        [Header("Options")]
        [SerializeField]
        private float baseSpeed;
        [SerializeField]
        private ProgressSnapOptions snapOptions;
        [SerializeField]
        private TextPosition textPosition;
        private bool smoothChange, snapText;
        float changeValue;
        
        
        /// <summary>
        /// Sets the progress bar to the given normalized value instantly.
        /// </summary>
        /// <param name="value">Must be in range [0,1]</param>
        public void ForceValue(float value)
        {
            changeValue = value;

            SetTextPosition();

            desiredValue = value;
        }

        /// <summary>
        /// The progress bar will move to the given value
        /// </summary>
        /// <param name="value">Must be in range [0,1]</param>
        /// <param name="speedOverride">Will override the base speed if one is given</param>
        public void SetTargetValue(float value, float? speedOverride = null)
        {
            SetTextPosition();

            if((value < desiredValue && snapOptions == ProgressSnapOptions.SnapToLowerValue) || (value > desiredValue && snapOptions == ProgressSnapOptions.SnapToHigherValue)){
                changeValue = value;
            }

            desiredValue = value;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            changeValue = Mathf.Lerp(changeValue, desiredValue, baseSpeed * Time.deltaTime);

            percentageText.text = Mathf.Round(changeValue * 100).ToString() + "%";
            percentageText.rectTransform.offsetMin = new Vector2(0, 0);

            fillBar.rectTransform.localScale = new Vector2(changeValue, 1f);

            if(snapText){
                percentageText.alignment = TMPro.TextAlignmentOptions.MidlineRight;
                float percentageWidth = (transform.parent.GetComponent<RectTransform>().sizeDelta.x * 0.5f/*420*/) * (1 - Mathf.Clamp(changeValue, 0.15f,1f));
                percentageText.rectTransform.offsetMax = new Vector2(-percentageWidth, 0);
            }
        }

        private void SetTextPosition(){
            percentageText.enabled = true;

            snapText = false;

            percentageText.alignment = TMPro.TextAlignmentOptions.CenterGeoAligned;

            switch (textPosition) 
            {
                case TextPosition.BarCenter:
                    percentageText.rectTransform.anchoredPosition3D = this.GetComponent<RectTransform>().anchoredPosition3D;
                    break;

                case TextPosition.Progress:
                    snapText = true;
                    break;

                //No Text
                default:
                    percentageText.enabled = false;
                    break;
            }
        }
    }
}
