using UnityEngine;
using UnityEngine.UI;

namespace NatCamU.Examples {

	[RequireComponent(typeof(Graphic))]
	public class RecordingIcon : MonoBehaviour {

		public bool IsRecording {get; set;}
		private float speed = 3.3f, time;
		private Graphic graphic;

		// Use this for initialization
		void Start () {
			// Cache the graphic
			graphic = GetComponent<Graphic>();
		}
		
		// Update is called once per frame
		void Update () {
			// Oscillate the color
			graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, IsRecording ? 0.5f * Mathf.Sin(speed * time + 1.5f * Mathf.PI) + 0.5f: 0f);
			// Increment time
			time = IsRecording ? time + Time.deltaTime : 0f;
		}
	}
}