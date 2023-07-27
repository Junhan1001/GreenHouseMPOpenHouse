using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Level3
{
    public class DrawLine : MonoBehaviour
    {
        public LineRenderer lr;

        ParticlesManager particlesManager;
        internal RectGrid rectGrid;

        internal bool finishedAddingPoints = false;

        public List<Vector2> points = new();

        private List<Vector3> vector3Points = new();

        private List<Vector2> reverseVector2Points = new();
        private List<Vector3> reverseVector3Points = new();

        [SerializeField]
        private float lineThickness = 0.3f;
        [SerializeField]
        internal Color lineColor = new Color(255, 255, 255);
        [SerializeField]
        private Material lineMat;

        [SerializeField]
        internal Transform lineFrom;
        [SerializeField]
        internal Transform lineTo;

        [SerializeField]
        internal bool isReverse = false;

        // Start is called before the first frame update
        void Start()
        {
            lr = GetComponent<LineRenderer>();
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.material = Instantiate(lineMat);
            lr.material.color = lineColor;

            particlesManager = GetComponent<ParticlesManager>();

            StartCoroutine(Draw());
        }

        private IEnumerator Draw()
        {
            while (finishedAddingPoints)
            {
                points.ForEach(x => vector3Points.Add(new Vector3(x.x, x.y, 0f)));
                reverseVector2Points = new(points);
                reverseVector2Points.Reverse();
                reverseVector3Points = new(vector3Points);
                reverseVector3Points.Reverse();
                lr.positionCount = vector3Points.Count;
                lr.SetPositions(vector3Points.ToArray());
                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                Mesh mesh = new();
                lr.BakeMesh(mesh);
                meshCollider.sharedMesh = mesh;
                particlesManager.targetPoints = points;
                particlesManager.SpawnParticle(points[0]);
                yield break;
            }
        }

        public void ResetCellToWalkable()
        {
            //points.ForEach(point => rectGrid.transform.Find("cell_" + point.x + "_" + point.y).GetComponent<RectGridCell>());

            int index = 0;
            points.ForEach((x) =>
            {
                Vector2 point = points[index];
                Transform cell = rectGrid.transform.Find("cell_" + point.x + "_" + point.y);
                if (cell)
                {
                    cell.GetComponent<RectGridCell>()?.SetWalkable();
                }
                index++;
            });
        }

        public bool isCorrect()
        {
            ComponentEvent component = lineFrom.GetComponentInParent<ComponentEvent>();
            // check if the line is connected to the correct target
            if (component.correctTarget.GetComponent<ComponentEvent>().specialID == lineTo.parent.GetComponent<ComponentEvent>().specialID)
            {
                // check if the line is the correct type
                if (component.GetComponent<ComponentEvent>().correctColor == lineColor)
                {
                    return true;
                }
            }
            else if (component.correctTarget2 ?
                component.correctTarget2.GetComponent<ComponentEvent>().specialID == lineTo.parent.GetComponent<ComponentEvent>().specialID : false)
            {
                if (component.GetComponent<ComponentEvent>().correctColor2 == lineColor)
                {
                    return true;
                }
            }
            return false;
        }

        public void ReverseLine()
        {
            isReverse = !isReverse;
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            particlesManager.StopSpawnParticle();
            Transform tempLineTo = lineFrom;
            lineFrom = lineTo;
            lineTo = tempLineTo;
            if (isReverse)
                particlesManager.targetPoints = reverseVector2Points;
            else
                particlesManager.targetPoints = points;
            particlesManager.SpawnParticle(particlesManager.targetPoints[0]);
        }
    }
}