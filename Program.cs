var builder = WebApplication.CreateBuilder(args);

// เปิดใช้งาน CORS เพื่อให้หน้าบ้านดึงข้อมูลไปใช้ได้
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("AllowAll");

// 1️⃣ MOCK DATA: จำลองข้อมูลโต๊ะ
var desks = new List<Desk>
{
    new Desk("A1", "Desk A1", true, ""),
    new Desk("A2", "Desk A2", true, ""),
    new Desk("A3", "Desk A3", false, "EMP001"),
    new Desk("B1", "Desk B1", true, "")
};

// 2️⃣ RESTful Endpoints
app.MapGet("/", () => "CNX-Space API is running perfectly! 🍌🚀");

// ✨ เพิ่มเส้นทางดึงข้อมูลโต๊ะทั้งหมด (แก้ 404 /api/desks)
app.MapGet("/api/desks", () => Results.Ok(desks));

// ✨ เพิ่มเส้นทาง Login (แก้ 404 /api/login)
app.MapPost("/api/login", (LoginRequest req) => 
{
    // สมมติว่ารหัสพนักงานที่เข้าได้คือ EMP001 หรือ EMP002
    if (req.employeeId == "EMP001" || req.employeeId == "EMP002") 
    {
        return Results.Ok(new { message = "เข้าสู่ระบบสำเร็จ", employeeId = req.employeeId });
    }
    return Results.BadRequest(new { message = "รหัสพนักงานไม่ถูกต้อง" });
});

app.MapPost("/api/book/{id}", (string id) => 
{
    var desk = desks.FirstOrDefault(d => d.Id.ToLower() == id.ToLower());
    
    if (desk == null) 
        return Results.NotFound(new { message = "ไม่พบโต๊ะที่ระบุ" });
        
    if (!desk.IsAvailable) 
        return Results.BadRequest(new { message = "โต๊ะนี้มีคนจองแล้วค่ะ" });

    desks.Remove(desk);
    // อัปเดตสถานะเป็นไม่ว่าง (สามารถเพิ่ม logic ใส่ชื่อคนจองได้ทีหลัง)
    desks.Add(desk with { IsAvailable = false, BookedBy = "EMP00X" });

    return Results.Ok(new { message = $"จองโต๊ะ {desk.Name} สำเร็จแล้ว!" });
});

// สั่งให้ระบบเช็กพอร์ตจากคลาวด์ ถ้าไม่มีค่อยใช้พอร์ตเริ่มต้น
var port = Environment.GetEnvironmentVariable("PORT") ?? "5112";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

// 🚨 Type declarations
// อัปเดต Desk ให้รองรับ bookedBy ด้วย
public record Desk(string Id, string Name, bool IsAvailable, string BookedBy);
public record LoginRequest(string employeeId);