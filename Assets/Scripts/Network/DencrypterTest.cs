using UnityEngine;
using System.Collections;

public class DencrypterTest : MonoBehaviour
{
    private const string encryptionKey = "Softv01**";

    public void Start()
    {
        //string test = "pedro quijada@gmail. cpm com";
        //string en = StringCipher.Encrypt(test, encryptionKey);
        //Debug.Log(en);
        Debug.Log(StringCipher.Decrypt("RiqdnQ+defd1VLvGt/Z+l+TYRP4mdoRSiDzmKus8q42vhkpu8mh0fdIsnFzK4mkqFQk+g0wm0tN+s4In9N5/Wit2k5eGd9segczCNsK1t6B5GTi4NOAWEgcJeINLydTZ", encryptionKey));
        //Debug.Log(StringCipher.Decrypt("WNm6AY+izXzWHbVDh9KAnPdmprI4ndyCqGx4yfCYN5DFd2gCXhgqw8lilwnUf38u2EIBO+J/2QOSdqTGMOTKPlH8vdyUJVHpduDVKH/sGvoB4fW3pIsy+l+J0EiqKmih", encryptionKey));
    }

}
