namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public static class VectorExtensions {
        public static Vector3d SwapYZ(this Vector3d vec) {
            double y = vec.y;
            vec.y = vec.z;
            vec.z = y;
            return vec;
        }
    }
}
