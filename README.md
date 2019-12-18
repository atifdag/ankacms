# AnkaCMS
Anka efsanevi bir kuştur. Doğu mitoloji ve efsanelerinde yer edinmiştir. Kimseye muhtaç olmamayı, kendi başına yaşamayı  seçtiği için kanaat sahibi olmayı, alçak gönüllülüğü ve sonsuzluğu temsil eder. Her şeye ve herkese eğilmez. Kimseye minnet etmez. Kaf Dağı gibi efsanevî bir yerde yaşar. 

Aynı zaman da projenin çıkış şehri Ankara'ya nispetle Anka ön eki kullanılmıştır :) 

CMS (Content Management System - İçerik Yönetim Sistemi) projenin ilk ürün hedefini ifade etmektedir. Bu nedenle AnkaCMS.
## Proje Yapısı
- Proje dokümanları "/doc" klasöründe,
- Projede kullanılan ham kodlar, muhtelif dosyalar ve materyaller "/res" klasöründe,
- Sunucu taraflı kaynak kodları "/src/server" klasöründe,
- İstemci taraflı kaynak kodları "/src/client" klasöründe
bulunmaktadır.
Projenin sunucu taraflı kodları ".NET Core" ile istemci-web taraflı kodları ise Angular ile hazırlanmaktadır.
## Projeyi Çalıştırma
Projeyi çalıştırmak için bilgisayarınızda ".Net Core", "git" ve "Node.js" kurulu olmalıdır. Eğer yoksa şu aşamaları takip edin:
1. https://git-scm.com/download adresinden kendi sisteminize uygun olan kurulum dosyasını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından aşağıdaki komutlarla git istemcisine kendinizi tanıtın.
```
git config --global user.name "Adınız ve Soyadınız"
git config --global user.email "epostaadresiniz@siteniz.com"
```
2. Komut satırından ```git --version``` komutuyla kurulumu test edin. Karşınızda git’in versiyonunu görüyorsanız git istemcisini başarılı bir şekilde kurdunuz demektir.
3. https://nodejs.org adresinden adresinden kendi sisteminize uygun olan kurulum dosyasını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından ```npm --version``` komutuyla kurulumu test edin. Karşınızda npm’in versiyonunu görüyorsanız kurulum başarılı demektir.
4. https://dotnet.microsoft.com/download/dotnet-core/current/runtime adresinden kendi sisteminize uygun olan ".NET Core Runtime" ve ".NET Core Desktop Runtime" kurulum dosyalarını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından ```dotnet --version``` komutuyla kurulumu test edin. Karşınızda dotnet’in versiyonunu görüyorsanız kurulum başarılı demektir.
5. Komut satırından ```git clone https://github.com/atifdag/ankacms.git``` komutuyla projeyi bilgisayarınıza indirin.
6. Komut satırından ```npm i -g @angular/cli``` komutuyla "Angular CLI" paketini indirip kurun.
7. Komut satırından "src/server/AnkaCMS.SetupConsoleApp" klasöründe iken ```dotnet run``` komutuyla veri tabanı kurulumunu yapın.
8. Komut satırından "src/server/AnkaCMS.WebApi" klasöründe iken ```dotnet run --urls="http://localhost:19001"``` komutuyla Web Api projesini çalıştırın.
9. Komut satırından "src/client/angular-web-admin" klasöründe iken ```npm i``` komutuyla bu projede kullanılan paketleri kurun.
10. Komut satırından "src/client/angular-web-admin" klasöründe iken ```ng s -o``` komutuyla Angular tabanlı web sitesi yönetim paneli projesini çalıştırın.
11. Komut satırından "src/client/angular-web-public" klasöründe iken ```npm i``` komutuyla bu projede kullanılan paketleri kurun.
12. Komut satırından "src/client/angular-web-public" klasöründe iken ```ng s -o``` komutuyla Angular tabanlı genel site projesini çalıştırın.
