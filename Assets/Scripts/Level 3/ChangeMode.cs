using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Level3
{
    public class ChangeMode : MonoBehaviour, IPointerClickHandler
    {
        public GameObject componentWheel;
        public GameObject lineWheel;

        [SerializeField]
        private bool select = true;
        [SerializeField]
        private bool draw = false;

        public Sprite drawLineSprite;
        public Sprite selectComponentSprite;

        public GameObject information;
        public GameObject btnUndo;

        public void OnPointerClick(PointerEventData eventData)
        {
            /// Whenever the change mode button is pressed, it will get the LinePathFind script and check if it is finding path.
            /// When is finding path, it will not allow the player to change the mode.
            /// Then, swap between select and draw mode.
            GameObject lineBoss = FindObjectOfType<LinePathFind>().gameObject;
            if (lineBoss.GetComponent<LinePathFind>().IsFindingPath())
                return;
            select = !select;
            draw = !draw;

            if (select)
            {
                // In select component mode, it will destroy all the lines. Then, change the wheel to component wheel.
                #region Old Code
                //txtMode.text = "Select Component";
                //FindObjectOfType<LineManagerController>().enabled = false;
                //var lineParents = GameObject.FindGameObjectsWithTag("LineParent").ToList();
                //var lines = GameObject.FindGameObjectsWithTag("Line").ToList();
                //GameObject lineBoss = FindObjectOfType<LineManagerController>().gameObject;
                //List<GameObject> allLines = new();
                //allLines.AddRange(lineParents);
                //allLines.AddRange(lines);
                //allLines.Remove(lineBoss);
                //allLines.ForEach(line => Destroy(line));

                //foreach (Transform child in playArea)
                //{
                //    if (child.CompareTag("Component"))
                //    {
                //        child.GetComponent<ComponentEvent>().ResetAllowDraw();
                //        child.GetComponent<ComponentEvent>().CorrectConnection = false;
                //    }
                //}
                #endregion
                GetComponent<Image>().sprite = selectComponentSprite;
                lineBoss.GetComponent<LinePathFind>().enabled = false;
                List<GameObject> allLines = new();
                for (int i = 0; i < lineBoss.transform.childCount; i++)
                {
                    allLines.Add(lineBoss.transform.GetChild(i).gameObject);
                }
                FindObjectsOfType<LineLimit>().ToList().ForEach(x => x.AllowDrawLine = true);
                allLines.ForEach(line => Destroy(line));

                FindObjectsOfType<DrawLine>().ToList().ForEach(x => x.ResetCellToWalkable());
                lineBoss.GetComponent<LinePathFind>().typeOfLineSelected = false;
                componentWheel.SetActive(true);
                lineWheel.SetActive(false);
                information.SetActive(false);
                btnUndo.SetActive(false);
            }
            else
            {
                // In draw mode, it will enable the LinePathFind and change the wheel to line wheel and show the information.

                //txtMode.text = "Draw Line";
                GetComponent<Image>().sprite = drawLineSprite;
                //FindObjectOfType<LineManagerController>().enabled = true;
                FindObjectOfType<LinePathFind>().enabled = true;
                componentWheel.SetActive(false);
                lineWheel.SetActive(true);
                information.SetActive(true);
                btnUndo.SetActive(true);
            }
        }

        public bool SelectComponent { get => select; }
        public bool DrawLine { get => draw; }
    }
}