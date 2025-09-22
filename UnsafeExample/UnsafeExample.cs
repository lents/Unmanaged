using System.Runtime.InteropServices;

namespace UnsafeExample
{
    public unsafe class UnsafeExample
    {
        public void DoSomethingUnsafe()
        {
            // Здесь можно использовать указатели
            int* ptr;
            // ...

            unsafe
            {
                int* intPtr = (int*)NativeMemory.Alloc(sizeof(int));
                *intPtr = 42; // Запись значения
                int value = *intPtr; // Чтение значения
                NativeMemory.Free(intPtr);
            }

            unsafe
            {
                // Fill example - initialize a buffer with zeros
                int bufferSize = 100;
                void* buffer = NativeMemory.Alloc((nuint)bufferSize);

                // Fill the entire buffer with zeros
                NativeMemory.Fill(buffer, (nuint)bufferSize, 0);

                // Copy example - copy data between buffers
                void* sourceBuffer = NativeMemory.Alloc(sizeof(long) * 5);
                void* destBuffer = NativeMemory.Alloc(sizeof(long) * 5);

                // Write some data to source buffer
                long* longArray = (long*)sourceBuffer;
                for (int i = 0; i < 5; i++)
                {
                    longArray[i] = i * 100;
                }

                // Copy the data from source to destination
                NativeMemory.Copy(sourceBuffer, destBuffer, (nuint)(sizeof(long) * 5));

                // Clean up
                NativeMemory.Free(buffer);
                NativeMemory.Free(sourceBuffer);
                NativeMemory.Free(destBuffer);
            }

            unsafe
            {
                // Int example
                void* intBuffer = NativeMemory.Alloc(sizeof(int));
                *((int*)intBuffer) = 42;
                int readInt = *((int*)intBuffer);
                NativeMemory.Free(intBuffer);

                // Float example
                void* floatBuffer = NativeMemory.Alloc(sizeof(float));
                *((float*)floatBuffer) = 3.14f;
                float readFloat = *((float*)floatBuffer);
                NativeMemory.Free(floatBuffer);

                // Working with arrays/blocks of memory
                int arraySize = 5;
                void* arrayBuffer = NativeMemory.Alloc((nuint)(sizeof(int) * arraySize));

                // Write to array
                int* intArray = (int*)arrayBuffer;
                for (int i = 0; i < arraySize; i++)
                {
                    intArray[i] = i * 10;
                }

                // Read from array
                for (int i = 0; i < arraySize; i++)
                {
                    Console.WriteLine($"Array[{i}] = {intArray[i]}");
                }

                NativeMemory.Free(arrayBuffer);
            }


        }

        public static void AnotherUnsafeMethod()
        {
            // Можно использовать unsafe блок внутри обычного метода
            unsafe
            {
                byte* buffer = (byte*)NativeMemory.Alloc(10);
                // ...
                NativeMemory.Free(buffer);
            }
        }
    }

}
