namespace Disposable
{
    public class BadResourceHolder
    {
        private FileStream _file;
        private string _filePath;

        public BadResourceHolder(string filePath)
        {
            _filePath = filePath;
            _file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Console.WriteLine($"[Bad] Resource acquired: {_filePath}");
        }

        public void WriteSomeData(string data)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data + Environment.NewLine);
            _file.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"[Bad] Data written to {_filePath}");
        }

        // Здесь нет Dispose() и нет финализатора.
        // Файл будет закрыт только при завершении приложения или когда GC по какой-то причине решит,
        // что FileStream больше не нужен, но это будет очень недетерминированно.
    }

}
