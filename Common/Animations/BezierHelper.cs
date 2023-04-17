namespace ImproveGame.Common.Animations;

public class BezierHelper
{
    /// <summary>
    /// 高阶贝塞尔曲线计算
    /// </summary>
    /// <param name="time"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static float Calculate(float time, params float[] array)
    {
        if (array.Length < 0)
        {
            return 0f;
        }

        if (array.Length == 1)
        {
            return array[0];
        }

        float[] newArray = new float[array.Length - 1];

        for (int i = 0; i < array.Length - 1; i++)
        {
            newArray[i] = (array[i + 1] - array[i]) * time;
        }

        return Calculate(time, newArray);
    }
}
