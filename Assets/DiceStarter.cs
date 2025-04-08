using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DiceStarter : MonoBehaviour
{
    [SerializeField] float maxLinearSpeed = 1000f;
    [SerializeField] float maxAngularSpeed = 1000f;
    [SerializeField] float textSize = 5f;
    [SerializeField] float showRotation = 0f;
    [SerializeField] Transform Origin;

    public Vector3 showPosition;

    Rigidbody rb;
    float freezeDelay = 1f;
    bool finalMoveStarted = false;
    Quaternion rotation;
    float timeToDestination = 0.1f;

    List<Vector3> pos = new List<Vector3>();
    List<Vector3> norm = new List<Vector3>();
    List<TextMeshPro> nums = new List<TextMeshPro>();

    private void Awake()
    {
        FindFaces();
        DrawNumbers();
        ApplyRandomRotation();
        ApplyRandomForce();
    }

    private void Update()
    {
        if (((rb.velocity.magnitude + rb.angularVelocity.magnitude) < 0.1f))
        {
            if(freezeDelay < 0f)
            {                
                if(!finalMoveStarted)
                {
                    rb.isKinematic = true;
                    GetComponent<Collider>().enabled = false;
                    GetComponent<AudioSource>().Play();

                    rotation = Quaternion.AngleAxis(showRotation - 30f,Vector3.right) * transform.rotation;
                    Origin.parent = transform.parent;
                    transform.parent = Origin;
                    finalMoveStarted = true;
                }
                foreach(TextMeshPro text in nums)
                {
                    text.transform.GetComponent<RotateNumbers>().RotateNums();
                }
                Origin.position = Vector3.Lerp(Origin.position, showPosition, Time.deltaTime * (1f / timeToDestination));
                Origin.rotation = Quaternion.Lerp(Origin.rotation, rotation, Time.deltaTime * (1f / timeToDestination));
            }
            freezeDelay -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        
    }

    void DrawNumbers()
    {
        for (int i = 0; i < norm.Count; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            obj.transform.position = pos[i] + norm[i] * 0.01f;
            obj.transform.LookAt(obj.transform.position - 10f * norm[i],Vector3.up);
            obj.AddComponent<RotateNumbers>();
            TextMeshPro textMesh = obj.AddComponent<TextMeshPro>();
            //textMesh.text = "<u>" + (i + 1).ToString() + "</u>";
            textMesh.text = (i + 1).ToString();
            textMesh.fontSize = textSize;
            textMesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.color = Color.green;
            nums.Add(textMesh);
        }
    }

    void FindFaces()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        int triangles = (int)(mesh.triangles.Length / 3f);
        Vector3[] positions = new Vector3[triangles];
        Vector3[] normals = new Vector3[triangles];

        for (int i = 0; i < triangles; i++)
        {
            Vector3 v1 = transform.localToWorldMatrix.MultiplyPoint3x4(mesh.vertices[mesh.triangles[3 * i + 0]]);
            Vector3 v2 = transform.localToWorldMatrix.MultiplyPoint3x4(mesh.vertices[mesh.triangles[3 * i + 1]]);
            Vector3 v3 = transform.localToWorldMatrix.MultiplyPoint3x4(mesh.vertices[mesh.triangles[3 * i + 2]]);
            positions[i] = (v1 + v2 + v3) / 3f;
            normals[i] = Vector3.Cross(v2 - v1, v3 - v1);
        }

        for (int i = 0; i < triangles; i++)
        {
            bool doubl = false;
            foreach(Vector3 n in norm)
            {
                doubl |= (n == normals[i]);
            }
            
            if(!doubl)
            {
                Vector3 center = positions[i];
                int num = 1;
                for (int w = 0; w < triangles; w++)
                {
                    if (i != w)
                    {
                        if (normals[i] == normals[w])
                        {
                            num++;
                            center += positions[w];
                        }
                    }
                }
                center = center / (float)num;
                norm.Add(normals[i]);
                pos.Add(center);
            }
        }
    }

    void ApplyRandomRotation()
    {
        Vector3 rot = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
        transform.rotation = Quaternion.Euler(rot);
    }

    void ApplyRandomForce()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 force = new Vector3(UnityEngine.Random.Range(-maxLinearSpeed, maxLinearSpeed), 0f, UnityEngine.Random.Range(-maxLinearSpeed, maxLinearSpeed));
        rb.AddForce(force);
        rb.AddTorque(Vector3.up * UnityEngine.Random.Range(-maxAngularSpeed, maxAngularSpeed));
        rb.AddTorque(Vector3.right * UnityEngine.Random.Range(-maxAngularSpeed, maxAngularSpeed));
        rb.AddTorque(Vector3.forward * UnityEngine.Random.Range(-maxAngularSpeed, maxAngularSpeed));
    }
}
