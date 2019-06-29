using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Vector3 RoundedVector(Vector3 vec)
    {
        return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
    }

    public static Vector3 VectorAbs(Vector3 vec) {
        return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
    }

    // an iterator for a rectangular prism region of blocks
    // example usage:
    //   foreach(Vector3 block in new BlockRegion(prismCorner1, prismCorner2)) {
    //     ...
    //   }
    public class BlockRegion : IEnumerable<Vector3>
    {
        private Vector3 _startPos;
        private Vector3 _endPos;

        public BlockRegion(Vector3 startPos, Vector3 endPos)
        {
            _startPos = Vector3.Min(startPos, endPos);
            _endPos = Vector3.Max(startPos, endPos);
            Debug.Log("Start pos: " + startPos);
            Debug.Log("End pos: " + endPos);
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            for (int x = Mathf.RoundToInt(_startPos.x); x <= Mathf.RoundToInt(_endPos.x); x++) {
                for (int y = Mathf.RoundToInt(_startPos.y); y <= Mathf.RoundToInt(_endPos.y); y++) {
                    for (int z = Mathf.RoundToInt(_startPos.z); z <= Mathf.RoundToInt(_endPos.z); z++) {
                        yield return new Vector3(x, y, z);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
