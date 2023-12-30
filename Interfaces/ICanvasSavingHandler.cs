namespace screenerWpf.Interfaces
{
    public interface ICanvasSavingHandler
    {
        // Metoda zapisująca zawartość płótna do pliku
        void SaveCanvasToFile();

        // Metoda zapisująca zawartość płótna do pliku szybko, zwracająca ścieżkę - do użytku przy eksporcie do chmury 
        string SaveCanvasToFileFast();

        // Metoda zapisująca zawartość płótna do pliku PDF
        void SaveCanvasToPdfFile();
    }
}
