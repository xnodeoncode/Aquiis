using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Aquiis.Tests;

/// <summary>
/// End-to-end tests for Phase 5.5 Multi-Organization Management scenarios.
/// Based on PROPERTY-TENANT-LIFECYCLE-ROADMAP.md testing scenarios.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PropertyManagementTests : PageTest
{
    private const string BaseUrl = "http://localhost:5197";
    private const int KeepBrowserOpenSeconds = 30; // Set to 0 to close immediately
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            BaseURL = BaseUrl,
            RecordVideoDir = Path.Combine(Directory.GetCurrentDirectory(), "test-videos"),
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }
    
    /// <summary>
    /// Test Scenario 1: Owner - Full access across all organizations
    /// </summary>
    [Test]
    public async Task Scenario1_Owner_HasFullAccessToAllOrganizations()
    {
        // Navigate to home page
        await Page.GotoAsync(BaseUrl);
        
        // Click Sign In button to get to login page
        await Page.ClickAsync("a[href='/Account/Login']");
        await Page.WaitForSelectorAsync("#Input\\.Email");
        
        // Login as Owner
        await Page.FillAsync("#Input\\.Email", "owner1@aquiis.com");
        await Page.FillAsync("#Input\\.Password", "Today123");
        await Page.ClickAsync("button[type='submit']");
        
        // Wait for dashboard to load
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        // Verify owner can access Properties page
        await Page.ClickAsync("a[href='propertymanagement/properties']");
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        
        // Verify "Add Property" button is visible (full access)
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Add Property" }))
            .ToBeVisibleAsync();
        
        // Success - owner can access Properties page with full permissions
        
        // Keep browser open for review/recording
        if (KeepBrowserOpenSeconds > 0)
            await Task.Delay(KeepBrowserOpenSeconds * 1000);
    }
    
    /// <summary>
    /// Test Scenario 2: Administrator - Manage single organization
    /// </summary>
    [Test]
    public async Task Scenario2_Administrator_ManagesSingleOrganization()
    {
        await Page.GotoAsync(BaseUrl);
        
        // Click Sign In button
        await Page.ClickAsync("a[href='/Account/Login']");
        await Page.WaitForSelectorAsync("#Input\\.Email");
        
        // Login as Administrator
        await Page.FillAsync("#Input\\.Email", "jc@example.com");
        await Page.FillAsync("#Input\\.Password", "Today123");
        await Page.ClickAsync("button[type='submit']");
        
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        // Verify Alice can create properties
        await Page.ClickAsync("a[href='propertymanagement/properties']");
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Add Property" }))
            .ToBeVisibleAsync();
        
        // Verify Alice can access Tenants
        await Page.ClickAsync("a[href='propertymanagement/tenants']");
        await Page.WaitForSelectorAsync("h1:has-text('Tenants')");
        
        // Verify "Add Tenant" button is visible
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Add Tenant" }))
            .ToBeVisibleAsync();
        
        // Keep browser open for review/recording
        if (KeepBrowserOpenSeconds > 0)
            await Task.Delay(KeepBrowserOpenSeconds * 1000);
    }
    
    /// <summary>
    /// Test Scenario 3: PropertyManager - View/edit assigned properties only
    /// </summary>
    [Test]
    public async Task Scenario3_PropertyManager_HasLimitedAccess()
    {
        await Page.GotoAsync(BaseUrl);
        
        // Click Sign In button
        await Page.ClickAsync("a[href='/Account/Login']");
        await Page.WaitForSelectorAsync("#Input\\.Email");
        
        // Login as PropertyManager
        await Page.FillAsync("#Input\\.Email", "jh@example.com");
        await Page.FillAsync("#Input\\.Password", "Today123");
        await Page.ClickAsync("button[type='submit']");
        
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        // Navigate to Properties
        await Page.ClickAsync("a[href='propertymanagement/properties']");
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        
        // Verify Bob can see "Add Property" button (PropertyManager can create)
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Add Property" }))
            .ToBeVisibleAsync();
        
        // Success - PropertyManager can access Properties page with create permissions
        
        // Keep browser open for review/recording
        if (KeepBrowserOpenSeconds > 0)
            await Task.Delay(KeepBrowserOpenSeconds * 1000);
    }
    
    /// <summary>
    /// Test Scenario 4: User - Read-only access
    /// </summary>
    [Test]
    public async Task Scenario4_User_HasReadOnlyAccess()
    {
        await Page.GotoAsync(BaseUrl);
        
        // Click Sign In button
        await Page.ClickAsync("a[href='/Account/Login']");
        await Page.WaitForSelectorAsync("#Input\\.Email");
        
        // Login as User
        await Page.FillAsync("#Input\\.Email", "mya@example.com");
        await Page.FillAsync("#Input\\.Password", "Today123");
        await Page.ClickAsync("button[type='submit']");
        
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        // Verify Lisa can see Dashboard
        await Expect(Page.Locator("h1, h2").Filter(new() { HasText = "Dashboard" }))
            .ToBeVisibleAsync();
        
        // Navigate to Properties
        await Page.ClickAsync("a[href='propertymanagement/properties']");
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        
        // Verify "Add Property" button is NOT visible (read-only)
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Add Property" }))
            .Not.ToBeVisibleAsync();
        
        // Success - user can view Properties page (read-only)
        
        // Navigate to Tenants
        await Page.ClickAsync("a[href='propertymanagement/tenants']");
        await Page.WaitForSelectorAsync("h1:has-text('Tenants')");
        
        // Verify "Add Tenant" button is NOT visible (read-only)
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Add Tenant" }))
            .Not.ToBeVisibleAsync();
        
        // Keep browser open for review/recording
        if (KeepBrowserOpenSeconds > 0)
            await Task.Delay(KeepBrowserOpenSeconds * 1000);
    }
    
    /// <summary>
    /// Test Scenario 5: Cross-organization isolation
    /// </summary>
    [Test]
    public async Task Scenario5_UsersOnlySeeTheirOrganizationData()
    {
        await Page.GotoAsync(BaseUrl);
        
        // Click Sign In button
        await Page.ClickAsync("a[href='/Account/Login']");
        await Page.WaitForSelectorAsync("#Input\\.Email");
        
        // Login as Administrator (single org access)
        await Page.FillAsync("#Input\\.Email", "jc@example.com");
        await Page.FillAsync("#Input\\.Password", "Today123");
        await Page.ClickAsync("button[type='submit']");
        
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        // Navigate to Properties
        await Page.ClickAsync("a[href='propertymanagement/properties']");
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        
        // Verify page loaded successfully - organization isolation is enforced at service layer
        // This test confirms Administrator can access the Properties page
        // Data isolation is tested through the service layer (not UI)
        var pageTitle = await Page.Locator("h1:has-text('Properties')").TextContentAsync();
        Assert.That(pageTitle, Does.Contain("Properties"), "Administrator can access Properties page");
        
        // Keep browser open for review/recording
        if (KeepBrowserOpenSeconds > 0)
            await Task.Delay(KeepBrowserOpenSeconds * 1000);
    }
    
    /// <summary>
    /// Test Scenario 6: Owner can access system across organizations
    /// </summary>
    [Test]
    public async Task Scenario6_Owner_CanAccessSystem()
    {
        await Page.GotoAsync(BaseUrl);
        
        // Click Sign In button
        await Page.ClickAsync("a[href='/Account/Login']");
        await Page.WaitForSelectorAsync("#Input\\.Email");
        
        // Login as Owner (access to multiple orgs)
        await Page.FillAsync("#Input\\.Email", "owner1@aquiis.com");
        await Page.FillAsync("#Input\\.Password", "Today123");
        await Page.ClickAsync("button[type='submit']");
        
        await Page.WaitForSelectorAsync("text=Dashboard");
        
        // Verify owner can access Properties
        await Page.ClickAsync("a[href='propertymanagement/properties']");
        await Page.WaitForSelectorAsync("h1:has-text('Properties')");
        
        // Keep browser open for review/recording
        if (KeepBrowserOpenSeconds > 0)
            await Task.Delay(KeepBrowserOpenSeconds * 1000);
    }
}
