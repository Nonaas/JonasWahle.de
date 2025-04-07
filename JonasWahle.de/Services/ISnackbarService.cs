namespace JonasWahle.de.Services
{
    public interface ISnackbarService
    {
        void ShowError(string errorMessage);
        void ShowInfo(string infoMessage);
        void ShowSuccess(string successMessage);
        void ShowWarning(string warningMessage);
    }
}