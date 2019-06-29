using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splice : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GL.wireframe = true;
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Run splice");
            RunSplice();
        }
    }

    Vector3 Intersect(Plane p, Vector3 p0, Vector3 p1) {
        Ray r = new Ray(p0, p1 - p0);
        float along;
        p.Raycast(r, out along);
        return r.GetPoint(along);
    }

    void RunSplice() {
        MeshFilter[] mesh_filters = FindObjectsOfType<MeshFilter>();
        Debug.Log("Found :" + mesh_filters.Length + " meshes");
        for(int m=0; m<mesh_filters.Length; m++) {
            GameObject obj = mesh_filters[m].gameObject;
            Debug.Log("Mesh: " + m);
            Mesh mesh = mesh_filters[m].mesh;
            Vector3[] vertices = mesh.vertices;
            int[] indices = mesh.triangles;
            Vector2[] uvs = mesh.uv;
            List<Vector3> vertices_to_add = new List<Vector3>();
            List<int> indices_to_add = new List<int>();
            List<Vector2> uvs_to_add = new List<Vector2>();
            Plane p = new Plane(transform.up, transform.position + new Vector3(.001f, .001f, .001f));

            for (int t=0; t<indices.Length / 3; t++) {
                int ia = indices[3*t];
                int ib = indices[3*t + 1];
                int ic = indices[3*t + 2];

                Vector3 a = vertices[ia];
                Vector3 b = vertices[ib];
                Vector3 c = vertices[ic];

                a = obj.transform.TransformPoint(a);
                b = obj.transform.TransformPoint(b);
                c = obj.transform.TransformPoint(c);

                bool sa = p.GetSide(a);
                bool sb = p.GetSide(b);
                bool sc = p.GetSide(c);

                //Debug.Log(sa);
                //Debug.Log(sb);
                //Debug.Log(sc);

                // Subdivide the straddling triangle.
                if(sa != sb || sa != sc || sb != sc) {
                    Debug.Log("Subdividing!");
                    Debug.Log("Plane pos: ");
                    Debug.Log(transform.position);
                    Debug.Log("Plane norm: ");
                    Debug.Log(transform.up);
                    Debug.Log("A: ");
                    Debug.Log(a);
                    Debug.Log("B: ");
                    Debug.Log(b);
                    Debug.Log("C: ");
                    Debug.Log(c);

                    Vector3 p0, p1, p2;
                    int i0, i1, i2;
                    if(sa == sb) {
                        p0 = a;
                        i0 = ia;
                        p1 = b;
                        i1 = ib;

                        p2 = c;
                        i2 = ic;
                    } else if (sb == sc) {
                        p0 = b;
                        i0 = ib;
                        p1 = c;
                        i1 = ic;

                        p2 = a;
                        i2 = ia;
                    } else {
                        p0 = c;
                        i0 = ic;
                        p1 = a;
                        i1 = ia;

                        p2 = b;
                        i2 = ib;
                    }

                    Debug.Assert(p.GetSide(p0) == p.GetSide(p1));
                    Debug.Assert(p.GetSide(p0) != p.GetSide(p2));
                    Debug.Assert(p.GetSide(p1) != p.GetSide(p2));

                    Vector3 a0 = Intersect(p, p0, p2);
                    Vector3 a1 = Intersect(p, p1, p2);

                    a0 = obj.transform.InverseTransformPoint(a0);
                    a1 = obj.transform.InverseTransformPoint(a1);

                    vertices_to_add.Add(a0);
                    vertices_to_add.Add(a1);

                    Vector2 uva0 = new Vector2(0, 0);
                    Vector2 uva1 = new Vector2(0, 0);
                    uvs_to_add.Add(uva0);
                    uvs_to_add.Add(uva1);

                    int a0i = vertices.Length + vertices_to_add.Count - 2;
                    int a1i = vertices.Length + vertices_to_add.Count - 1;

                    indices[3*t] = i2;
                    indices[3*t + 1] = a0i;
                    indices[3*t + 2] = a1i;

                    indices_to_add.Add(a0i);
                    indices_to_add.Add(i0);
                    indices_to_add.Add(i1);

                    indices_to_add.Add(i1);
                    indices_to_add.Add(a1i);
                    indices_to_add.Add(a0i);
                    /*
                    p0 = obj.transform.InverseTransformPoint(p0);
                    p1 = obj.transform.InverseTransformPoint(p1);
                    p2 = obj.transform.InverseTransformPoint(p2);

                    Vector3[] t0 = {a0, p0, p1};
                    Vector3[] t1 = {p1, a1, a0};
                    Vector3[] t2 = {p2, a0, a1};

                    vertices[i0] = t0[0];
                    vertices[i1] = t0[1];
                    vertices[i2] = t0[2];

                    to_add.AddRange(t1);
                    to_add.AddRange(t2);
                    */
                }
            }

            if(vertices_to_add.Count > 0) {
                List<Vector3> vertex_list = new List<Vector3>();
                vertex_list.AddRange(vertices);
                vertex_list.AddRange(vertices_to_add);
                vertices = vertex_list.ToArray();

                List<int> index_list = new List<int>();
                index_list.AddRange(indices);
                index_list.AddRange(indices_to_add);
                indices = index_list.ToArray();
            }
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
        }


        // 2. Project interior triangles onto midplane
    }
}
