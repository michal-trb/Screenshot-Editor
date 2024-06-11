namespace screenerWpf.Interfaces
{
    public interface ICanvasSavingHandler
    {
        void SaveCanvasToFile();
        string SaveCanvasToFileFast();
        void SaveCanvasToPdfFile();
    }
}
