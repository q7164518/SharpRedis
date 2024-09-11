namespace SharpRedis
{
    public interface IBitFieldArgOverflow
    {
        /// <summary>
        /// Wrap around, both with signed and unsigned integers. In the case of unsigned integers, wrapping is like performing the operation modulo the maximum value the integer can contain (the C standard behavior). With signed integers instead wrapping means that overflows restart towards the most negative value and underflows towards the most positive ones, so for example if an i8 integer is set to the value 127, incrementing it by 1 will yield -128
        /// <para>环绕，有符号和无符号整数。对于无符号整数，换行就像执行对整数可以包含的最大值取模的运算（C 标准行为）。使用有符号整数而不是换行意味着上溢重新开始朝向最大负值，下溢朝向最大正值，因此例如，如果 i8 整数设置为值 127，则将其递增 1 将产生 -128 </para>
        /// </summary>
        /// <returns></returns>
        IBitFieldArg Wrap();

        /// <summary>
        /// Uses saturation arithmetic, that is, on underflows the value is set to the minimum integer value, and on overflows to the maximum integer value. For example incrementing an i8 integer starting from value 120 with an increment of 10, will result into the value 127, and further increments will always keep the value at 127. The same happens on underflows, but towards the value is blocked at the most negative value
        /// <para>使用饱和算术，即下溢时将值设置为最小整数值，上溢时将值设置为最大整数值。例如，从值 120 开始递增 i8 整数，增量为 10，将得到值 127，进一步的增量将始终使该值保持在 127。下溢时也会发生同样的情况，但下溢的方向是该值被阻止在最大负值</para>
        /// </summary>
        /// <returns></returns>
        IBitFieldArg Sat();

        /// <summary>
        /// In this mode no operation is performed on overflows or underflows detected. The corresponding return value is set to NULL to signal the condition to the caller
        /// <para>在此模式下，检测到上溢或下溢时不执行任何操作。相应的返回值设置为 NULL 以向调用者发出信号通知</para>
        /// </summary>
        /// <returns></returns>
        IBitFieldArg Fail();
    }
}
