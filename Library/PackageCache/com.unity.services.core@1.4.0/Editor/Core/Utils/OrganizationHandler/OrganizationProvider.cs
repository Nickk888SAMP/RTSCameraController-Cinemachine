namespace Unity.Services.Core.Editor.OrganizationHandler
{
    public static class OrganizationProvider
    {
        static IOrganizationHandler s_OrganizationHandler = new OrganizationHandler();

        public static IOrganizationHandler Organization
        {
            get => s_OrganizationHandler;
            set
            {
                if (value != null)
                {
                    s_OrganizationHandler = value;
                }
            }
        }
    }
}
