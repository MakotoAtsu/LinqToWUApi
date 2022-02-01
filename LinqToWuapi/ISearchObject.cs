namespace LinqToWuapi
{
    public enum sUpdateType
    {
        Software,
        Driver,
    }

    public enum sDeploymentAction
    {
        None,
        Installation,
        Uninstallation,
        Detection,
        OptionalInstallation
    }

    public class sCategoryIDs
    {
        public bool Contains(string UUID)
        {
            return default;
        }
    }

    /// <summary>
    /// Search Object for windows update item. Include all attribute support for search.
    /// 
    /// The '&&' and '||' operators can be used to connect multiple criteria. 
    /// However, '||' can be used only at the top level of the search criteria. 
    /// Therefore, "(x == True and y == True) or (z == True)" is valid, 
    /// but "(x == True) and (y == True || z == True)" is not valid.
    /// </summary>
    public interface ISearchObject
    {

        /// <summary>
        /// Finds updates of a specific type, such as "'Driver'" and "'Software'".
        /// </summary>
        sUpdateType Type { get; set; }

        /// <summary>
        /// Finds updates that are deployed for a specific action, such as an installation or uninstallation that the administrator of a server specifies.
        /// "DeploymentAction='Installation'" finds updates that are deployed for installation on a destination computer. "DeploymentAction='Uninstallation'" depends on the other query criteria.
        /// "DeploymentAction='Uninstallation'" finds updates that are deployed for uninstallation on a destination computer. "DeploymentAction='Uninstallation'" depends on the other query criteria.
        /// If this criterion is not explicitly specified, each group of criteria that is joined to an AND operator implies "DeploymentAction='Installation'".
        /// </summary>
        sDeploymentAction DeploymentAction { set; get; }

        /// <summary>
        /// Finds updates that are intended for deployment by Automatic Updates.
        /// "IsAssigned=True" finds updates that are intended for deployment by Automatic Updates, which depends on the other query criteria.At most, one assigned Windows-based driver update is returned for each local device on a destination computer.
        /// "IsAssigned=False" finds updates that are not intended to be deployed by Automatic Updates.
        /// </summary>
        bool IsAssigned { get; set; }

        /// <summary>
        /// "BrowseOnly=True" finds updates that are considered optional.
        /// "BrowseOnly=False" finds updates that are not considered optional.
        /// </summary>
        bool BrowseOnly { get; set; }

        /// <summary>
        /// Finds updates where the AutoSelectOnWebSites property has the specified value.
        /// "AutoSelectOnWebSites=True" finds updates that are flagged to be automatically selected by Windows Update.
        /// "AutoSelectOnWebSites=False" finds updates that are not flagged for Automatic Updates.
        /// </summary>
        bool AutoSelectOnWebSites { get; set; }

        /// <summary>
        /// Finds updates for which the value of the UpdateIdentity.UpdateID property matches the specified value. Can be used with the != operator to find all the updates that do not have an UpdateIdentity.UpdateID of the specified value.
        /// For example, "UpdateID='12345678-9abc-def0-1234-56789abcdef0'" finds updates for UpdateIdentity.UpdateID that equal 12345678-9abc-def0-1234-56789abcdef0.
        /// For example, "UpdateID!='12345678-9abc-def0-1234-56789abcdef0'" finds updates for UpdateIdentity.UpdateID that are not equal to 12345678-9abc-def0-1234-56789abcdef0.
        /// Note : A RevisionNumber clause can be combined with an UpdateID clause that contains an = (equal) operator. However, the RevisionNumber clause cannot be combined with an UpdateID clause that contains the != (not-equal) operator.
        /// For example, "UpdateID='12345678-9abc-def0-1234-56789abcdef0' and RevisionNumber=100" can be used to find the update for UpdateIdentity.UpdateID that equals 12345678-9abc-def0-1234-56789abcdef0 and whose UpdateIdentity.RevisionNumber equals 100.
        /// </summary>
        string UpdateID { get; set; }

        /// <summary>
        /// Finds updates for which the value of the UpdateIdentity.RevisionNumber property matches the specified value.
        /// For example, "RevisionNumber=2" finds updates where UpdateIdentity.RevisionNumber equals 2.
        /// This criterion must be combined with the UpdateID property.
        /// </summary>
        int RevisionNumber { get; set; }

        /// <summary>
        /// Finds updates that belong to a specified category.
        /// Only Support 'Contains' operator.  
        /// </summary>
        sCategoryIDs CategoryIDs { get; set; }

        /// <summary>
        /// Finds updates that are installed on the destination computer.
        /// "IsInstalled=True" finds updates that are installed on the destination computer.
        /// "IsInstalled=False" finds updates that are not installed on the destination computer.
        /// </summary>
        bool IsInstalled { get; set; }

        /// <summary>
        /// Finds updates that are marked as hidden on the destination computer.
        /// "IsHidden=True" finds updates that are marked as hidden on a destination computer.When you use this clause, you can set the UpdateSearcher.IncludePotentiallySupersededUpdates property to VARIANT_TRUE so that a search returns the hidden updates. The hidden updates might be superseded by other updates in the same results.
        /// "IsHidden=False" finds updates that are not marked as hidden.If the UpdateSearcher.IncludePotentiallySupersededUpdates property is set to VARIANT_FALSE, it is better to include that clause in the search filter string so that the updates that are superseded by hidden updates are included in the search results.VARIANT_FALSE is the default value.
        /// </summary>
        bool IsHidden { get; set; }

        /// <summary>
        /// When set to True, finds updates that are present on a computer.
        /// "IsPresent=True" finds updates that are present on a destination computer.If the update is valid for one or more products, the update is considered present if it is installed for one or more of the products.
        /// "IsPresent=False" finds updates that are not installed for any product on a destination computer.
        /// </summary>
        bool IsPresent { get; set; }

        /// <summary>
        /// Finds updates that require a computer to be restarted to complete an installation or uninstallation.
        /// "RebootRequired=True" finds updates that require a computer to be restarted to complete an installation or uninstallation.
        /// "RebootRequired=False" finds updates that do not require a computer to be restarted to complete an installation or uninstallation.
        /// </summary>
        bool RebootRequired { get; set; }
    }
}
