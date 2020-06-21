# apsiyon.logger
Apsiyon Logger

**Kullanımı:**

- **Apsiyon.Logger.Core** projesi, ILogger&#39;ın tüketileceği projeye bağımlılık olarak eklenir edilir.
- Ardından **Startup.ConfigureServices** altından service collection aracılığı ile **UseApsiyonLogger()** çağırılır.

publicvoid ConfigureServices(IServiceCollection services)

{

**services.UseApsiyonLogger();**

}

- Apsiyon Logger&#39;ın ayarları giriş projesindeki **appsettings(Developement).json** aracılığı ile yapılmaktadır.

**appsettings.json**

{

&quot;ApsiyonLogger&quot;: {

&quot;SQLServer&quot;: {

&quot;Active&quot;: true,

&quot;ConnectionString&quot;: &quot;Server=localhost;Database=Apsiyon;User Id=sa;Password=1;pooling=true;MultipleActiveResultSets=true;&quot;,

&quot;MigrationHistoryTableName&quot;: &quot;\_ApsiyonLoggerTableMigrationHistory&quot;

},

&quot;File&quot;: {

&quot;Active&quot;: true,

&quot;Path&quot;: &quot;logs/[machinename]/[today]/abc&quot;,

&quot;LogName&quot;: &quot;[today]-[ticks].json&quot;

},

&quot;Sentry&quot;: {

&quot;Active&quot;: true,

&quot;DSN&quot;: &quot;https://0783d48cac1d445890edbe2ab240907b@o254391.ingest.sentry.io/5283170&quot;

}

}

}

**appsettings Açıklaması:**

- In-built olarak 3 provider yeralmaktadır. SQLServer, File ve Sentry.
- Active değeri sadece true olan providerlar loglama yapabilmektedir.
- File provider&#39;ında **Path** ve **LogName** alanları köşeli parantezli ifade alır. Bu ifadeler runtime&#39;da otomatik olarak parse edilir. Örneğin [today] ifadesi [yyyyMMdd] =\&gt; &quot;20200620&quot; olarak yer değiştirir. Bu sayede log dosyalarına günlere kategorize etmek mümkün olmaktadır. [machinename] ifadesi ise işletim sisteminin kurulu olduğu makina ismini getirmektedir. Microservice mimarisinde, uzak dosya sunucusuna ağ üzerinden yazım işlemlerinde log&#39;un hangi sunucu/service&#39;den geldiği anlaşılabilecektir. [ticks] ise unix time&#39;dır.
- Sentry provider&#39;ı bulut loglamadır. Sentry.IO&#39;nun API wrapper&#39;ı entegre edildi.
- SQLServer provider&#39;ı &quot; **MigrationHistoryTableName**&quot; alanı yer almaktadır. Bu değer eğer SQL Server veritabanı hem loglama hem de ana projenin veritabanı olarak kullanılacaksa, Migration History tablosunun birbirine karışmaması için kullanılmaktadır.

- Apsiyon Logger&#39;ın tüketilmesi gereken noktada dependency injection yöntemiyle çağırılıp kullanılabilir.

publicclassHomeController : Controller

{

**private**** readonly **** ILogger \_logger;**

publicHomeController( **ILogger logger** )

{

**\_logger = logger;**

}

publicvoid Index(CancellationToken cancellationToken)

{

\_logger.Add(&quot;test&quot;, DateTime.Now);

\_logger.Add(&quot;test2&quot;, Environment.CurrentDirectory);

\_logger.Add(&quot;test3&quot;, Environment.OSVersion);

\_logger.Add(&quot;test4&quot;, null);

}

}

**Kullanılan Yazılım Desenleri:**

**Repository Design Pattern + UnitOfWork:** Verinin bir noktadan diğer uç noktaya taşınması için kullanıldı.

**Chain of Responsibility:** Hangi loglama provider&#39;ının kullanılmasına karar verilmesi için kullanıldı. Aynı zamanda loglama provider&#39;larında fallback tekniği uygulanıyor.

**Fallback Order:** SQLServer -\&gt; File -\&gt; Sentry

**Çalışma Stratejisi:**

1. Proje ilk çalıştırıldığında SQLServer provider&#39;ı tanımlı ve aktif ise kendine özel veritabanı tablosunu SQL Server üzerine yerleştirir. _(Apsiyon.Logger.Core. ApsiyonLogger.UseApsiyonLogger)_
2. Dependency injection ile dahil edilen ILogger üzerinden loglama isteği gönderilmesi durumunda loglama isteği kuyruğa alınır. _(Apsiyon.Logger.Service. Logger.Add)_
3. Kuyruk, sürekli çalışan bir HostedService tarafından işlenmektedir. _(Apsiyon.Logger.Service. HostedServices.QueueHostedService.__Worker)_
4. İşlenme sırası gelen loglama isteği ele alınır ve Apsiyon.Logger.Service.LogInjectorService.Fire metodu aracılığı ile işleme alınır.
5. Öncelikli olarak **SQLServer** denenir. SQL Server veritabanı tablosuna log kayıt altına alınırken, Object nesnesi byte array&#39;e çevirilerek binary formatında tutulur. Object&#39;in tipi ise ObjectType alanına yazdırılmaktadır.
6. SQLServer provider&#39;ının başarısız olması durumunda **File** provider&#39;ı devreye girer. Log dosyası belirtilen dosya yolunda belirtilen isimde JSON çıktısı olarak tutulur.
7. File provider&#39;ının başarısız olması durumunda **Sentry** provider&#39;ı devreye girer. Log detayları, JSON&#39;a çevirilerek Sentry.IO&#39;ya iletilir.
