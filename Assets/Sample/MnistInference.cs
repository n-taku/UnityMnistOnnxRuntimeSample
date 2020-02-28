using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using UnityEngine;

public class MnistInference
{
    private readonly InferenceSession session;

    public MnistInference(string modelPath)
    {
        session = new InferenceSession(modelPath);
    }

    //推論
    //Mnistは28x28のfloat値(0~1)のinputで推論できる，左上が原点で右下に向かう座標系
    public int Inference(float[] input_floats)
    {
        //推論する
        var scores = InferenceOnnx(input_floats);

        //最大のIndexを求める．Indexが推論した数字
        var maxScore = float.MinValue;
        int maxIndex = 0;
        for (int i = 0; i < scores.Length; i++)
        {
            float score = scores[i];
            if (maxScore < score)
            {
                maxScore = score;
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    private float[] InferenceOnnx(float[] input)
    {
        var inputName = session.InputMetadata.First().Key;
        var inputDim = session.InputMetadata.First().Value.Dimensions;
        var inputTensor = new DenseTensor<float>(new System.Memory<float>(input), inputDim);

        // OnnxRuntimeでの入力形式であるNamedOnnxValueを作成する
        var inputOnnxValues = new List<NamedOnnxValue> {
            NamedOnnxValue.CreateFromTensor (inputName, inputTensor)
        };

        // 推論を実行
        var results = session.Run(inputOnnxValues);
        var scores = results.First().AsTensor<float>().ToArray();

        return scores;
    }
}
