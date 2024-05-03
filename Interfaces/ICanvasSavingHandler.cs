namespace screenerWpf.Interfaces
{
    public interface ICanvasSavingHandler
    {
        void SaveCanvasToFile();

        // Metoda zapisująca zawartość płótna do pliku szybko, zwracająca ścieżkę - do użytku przy eksporcie do chmury 
        string SaveCanvasToFileFast();

        void SaveCanvasToPdfFile();
    }
}
