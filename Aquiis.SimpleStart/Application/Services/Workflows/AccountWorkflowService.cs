
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.SimpleStart.Application.Services;
using Aquiis.SimpleStart.Shared.Services;
using Aquiis.SimpleStart.Infrastructure.Data;

namespace Aquiis.SimpleStart.Application.Services.Workflows
{
    public enum AccountStatus
    {
        Created,
        Active,
        Locked,
        Closed
    }
    public class AccountWorkflowService : BaseWorkflowService, IWorkflowState<AccountStatus>
    {
        public AccountWorkflowService(ApplicationDbContext context,
            UserContextService userContext,
            NotificationService notificationService)
            : base(context, userContext)
        {
        }
        // Implementation of the account workflow service
        public string GetInvalidTransitionReason(AccountStatus fromStatus, AccountStatus toStatus)
        {
            throw new NotImplementedException();
        }

        public List<AccountStatus> GetValidNextStates(AccountStatus currentStatus)
        {
            throw new NotImplementedException();
        }

        public bool IsValidTransition(AccountStatus fromStatus, AccountStatus toStatus)
        {
            throw new NotImplementedException();
        }
    }
}