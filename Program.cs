var builder = WebApplication.CreateBuilder(args);

// เปิดใช้งาน CORS เพื่อให้หน้าบ้านดึงข้อมูลไปใช้ได้
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("AllowAll");

// 1️⃣ MOCK DATA: จำลองข้อมูลโต๊ะ (เรียกใช้งาน Desk ได้เลย)
var desks = new List<Desk>
{
    new Desk("A1", "Desk A1", true),
    new Desk("A2", "Desk A2", true),
    new Desk("A3", "Desk A3", false),
    new Desk("B1", "Desk B1", true)
};

// 2️⃣ RESTful Endpoints
app.MapGet("/api/desks", () => Results.Ok(desks));

app.MapPost("/api/book/{id}", (string id) => 
{
    var desk = desks.FirstOrDefault(d => d.Id.ToLower() == id.ToLower());
    
    if (desk == null) 
        return Results.NotFound(new { message = "ไม่พบโต๊ะที่ระบุ" });
        
    if (!desk.IsAvailable) 
        return Results.BadRequest(new { message = "โต๊ะนี้มีคนจองแล้วค่ะ" });

    desks.Remove(desk);
    desks.Add(desk with { IsAvailable = false });

    return Results.Ok(new { message = $"จองโต๊ะ {desk.Name} สำเร็จแล้ว!" });
});

// สั่งให้ระบบเช็กพอร์ตจากคลาวด์ ถ้าไม่มีค่อยใช้พอร์ตเริ่มต้น
var port = Environment.GetEnvironmentVariable("PORT") ?? "5112";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

// 🚨 ย้ายมาอยู่บรรทัดสุดท้ายตรงนี้แล้ว! (Type declarations must be at the end)
public record Desk(string Id, string Name, bool IsAvailable);