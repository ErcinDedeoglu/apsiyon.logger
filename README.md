# apsiyon.logger
Apsiyon Logger

Kullanımı: 
•	Apsiyon.Logger.Core projesi, ILogger’ın tüketileceği projeye bağımlılık olarak eklenir edilir.
•	Ardından Startup.ConfigureServices altından service collection aracılığı ile UseApsiyonLogger() çağırılır.
public void ConfigureServices(IServiceCollection services)
{
     services.UseApsiyonLogger();
}
•	Apsiyon Logger’ın ayarları giriş projesindeki appsettings(Developement).json aracılığı ile yapılmaktadır.
appsettings.json
{
  "ApsiyonLogger": {
    "SQLServer": {
      "Active": true,
      "ConnectionString": "Server=localhost;Database=Apsiyon;User Id=sa;Password=1;pooling=true;MultipleActiveResultSets=true;",
      "MigrationHistoryTableName": "_ApsiyonLoggerTableMigrationHistory"
    },
    "File": {
      "Active": true,
      "Path": "logs/[machinename]/[today]/abc",
      "LogName": "[today]-[ticks].json"
    },
    "Sentry": {
      "Active": true,
      "DSN": "https://0783d48cac1d445890edbe2ab240907b@o254391.ingest.sentry.io/5283170"
    }
  }
}
	appsettings Açıklaması:
-	In-built olarak 3 provider yeralmaktadır. SQLServer, File ve Sentry.
-	Active değeri sadece true olan providerlar loglama yapabilmektedir.
-	File provider’ında Path ve LogName alanları köşeli parantezli ifade alır. Bu ifadeler runtime’da otomatik olarak parse edilir. Örneğin [today] ifadesi [yyyyMMdd] => “20200620” olarak yer değiştirir. Bu sayede log dosyalarına günlere kategorize etmek mümkün olmaktadır. [machinename] ifadesi ise işletim sisteminin kurulu olduğu makina ismini getirmektedir. Microservice mimarisinde, uzak dosya sunucusuna ağ üzerinden yazım işlemlerinde log’un hangi sunucu/service’den geldiği anlaşılabilecektir. [ticks] ise unix time’dır.
-	Sentry provider’ı bulut loglamadır. Sentry.IO’nun API wrapper’ı entegre edildi. 
-	SQLServer provider’ı “MigrationHistoryTableName” alanı yer almaktadır. Bu değer eğer SQL Server veritabanı hem loglama hem de ana projenin veritabanı olarak kullanılacaksa, Migration History tablosunun birbirine karışmaması için kullanılmaktadır.
•	Apsiyon Logger’ın tüketilmesi gereken noktada dependency injection yöntemiyle çağırılıp kullanılabilir.
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger logger)
        {
            _logger = logger;
        }

        public void Index(CancellationToken cancellationToken)
        {
                          _logger.Add("test", DateTime.Now);
            _logger.Add("test2", Environment.CurrentDirectory);
            _logger.Add("test3", Environment.OSVersion);
            _logger.Add("test4", null);
        }
    }
Kullanılan Yazılım Desenleri:
Repository Design Pattern + UnitOfWork: Verinin bir noktadan diğer uç noktaya taşınması için kullanıldı.
Chain of Responsibility: Hangi loglama provider’ının kullanılmasına karar verilmesi için kullanıldı. Aynı zamanda loglama provider’larında fallback tekniği uygulanıyor.
Fallback Order: SQLServer -> File -> Sentry

Çalışma Stratejisi:
1.	Proje ilk çalıştırıldığında SQLServer provider’ı tanımlı ve aktif ise kendine özel veritabanı tablosunu SQL Server üzerine yerleştirir. (Apsiyon.Logger.Core. ApsiyonLogger.UseApsiyonLogger)
2.	Dependency injection ile dahil edilen ILogger üzerinden loglama isteği gönderilmesi durumunda loglama isteği kuyruğa alınır. (Apsiyon.Logger.Service. Logger.Add)
3.	Kuyruk, sürekli çalışan bir HostedService tarafından işlenmektedir. (Apsiyon.Logger.Service. HostedServices.QueueHostedService.Worker)
4.	İşlenme sırası gelen loglama isteği ele alınır ve Apsiyon.Logger.Service.LogInjectorService.Fire metodu aracılığı ile işleme alınır. 
5.	Öncelikli olarak SQLServer denenir. SQL Server veritabanı tablosuna log kayıt altına alınırken, Object nesnesi byte array’e çevirilerek binary formatında tutulur. Object’in tipi ise ObjectType alanına yazdırılmaktadır.
6.	SQLServer provider’ının başarısız olması durumunda File provider’ı devreye girer. Log dosyası belirtilen dosya yolunda belirtilen isimde JSON çıktısı olarak tutulur.
7.	File provider’ının başarısız olması durumunda Sentry provider’ı devreye girer. Log detayları, JSON’a çevirilerek Sentry.IO’ya iletilir.
