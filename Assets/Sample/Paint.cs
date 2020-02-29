using UnityEngine;
using UnityEngine.UI;

public class Paint : MonoBehaviour
{
    const int width = 28;
    const int height = 28;

    public MeshRenderer m;
    public Button b;
    public Text answer;
    
    private Texture2D texture;
    private readonly Color[] buffer = new Color[width * height];
    private readonly float[] input = new float[width * height];
    private MnistInference mnistInference;

    private void Start()
    {
        mnistInference = new MnistInference(Application.dataPath + "/MLModel/model.onnx");
        texture = new Texture2D(width, height);
        m.material.mainTexture = texture;
        ClearBuffer();
    }

    private void Update()
    {
        //クリック座標を白く塗りつぶす
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100.0f))
            {
                var pixel = new Vector2(hit.textureCoord.x * width, hit.textureCoord.y * height);
                Draw(Vector2Int.RoundToInt(pixel));
            }

            texture.SetPixels(buffer);
            texture.Apply();
        }

        //塗りつぶした部分を1としてfloatの配列に代入
        for (int i = 0; i < buffer.Length; i++)
        {
            input[i] = buffer[i].r;
        }

        //推論
        var num = mnistInference.Inference(input);
        answer.text = num.ToString();
    }

    //４ピクセル塗る
    private void Draw(Vector2Int p)
    {
        DrawBuffer(p.x, p.y);
        DrawBuffer(p.x + 1, p.y);
        DrawBuffer(p.x + 1, p.y + 1);
        DrawBuffer(p.x, p.y + 1);
    }

    //バッファーに書き込む
    private void DrawBuffer(int x, int y)
    {
        if (x < 0 || width <= x || y < 0 || height <= y)
        {
            return;
        }

        buffer.SetValue(Color.white, x + width * y);
    }

    //黒(0)で塗りつぶしてクリア
    public void ClearBuffer()
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = Color.black;
        }

        texture.SetPixels(buffer);
        texture.Apply();
    }
}
