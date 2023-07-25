using Microsoft.EntityFrameworkCore;
using SearchGallery.Persistence;
using SearchGallery.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SearchGalleryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SearchGalleryDb")!, b =>
    {
        b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        b.CommandTimeout(60 * 10);
    })
);

builder.Services.AddScoped<SearchGalleryDbContext>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection("StorageConfiguration"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.Run();
