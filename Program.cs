using EF10_QueryCache;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var meterListener = new MeterListenerHelper();

var con = new SqliteConnection("Data Source=file:memdb1?mode=memory&cache=shared");
await con.OpenAsync();

await AppDbContext.InitAndSeedDb(con);

/* add some delay for the metrics to hook up */
await Task.Delay(1000);

using var dbCtx = new AppDbContext(con, Guid.NewGuid());

_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().ToListAsync(); // cache miss (correct)
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().ToListAsync(); // cache hit
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().ToListAsync(); // cache hit
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().ToListAsync(); // cache hit
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().ToListAsync(); // cache hit

_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters().ToListAsync(); // cache miss (correct)
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters().ToListAsync(); // cache hit
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters().ToListAsync(); // cache hit
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters().ToListAsync(); // cache hit
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters().ToListAsync(); // cache hit

_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters(["TenantFilter"]).ToListAsync(); // cache miss (correct)
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters(["TenantFilter"]).ToListAsync(); // cache miss (why???)
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters(["TenantFilter"]).ToListAsync(); // cache miss (why???)
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters(["TenantFilter"]).ToListAsync(); // cache miss (why???)
_ = await dbCtx.Set<ProductEntity>().AsNoTrackingWithIdentityResolution().IgnoreQueryFilters(["TenantFilter"]).ToListAsync(); // cache miss (why???)

meterListener.CollectMetrics();

Console.WriteLine("Press Enter to exit...");
Console.ReadLine();