# AnkaCMS
Anka efsanevi bir kuştur. Doğu mitoloji ve efsanelerinde yer edinmiştir. Anka'nın en yaygın özelliği sonsuzluğu, kimseye muhtaç olmadan kendi başına yaşadığı için kanaati temsil etmesidir. Kaf Dağı gibi efsanevî bir yerde yaşar. Kanaat sahibi ve alçak gönüllü, her şeye ve herkese eğilmeyen, kimseye minnet etmeyen, uzlete çekilmiş kişileri ifade eder. Aynı zaman da projenin çıkış şehri Ankara'ya nispetle Anka ön eki kullanılmıştır :) CMS (Content Management System - İçerik Yönetim Sistemi) projenin ilk ürün hedefini ifade etmektedir. Bu nedenle AnkaCMS.
Projeyi çalıştırmak için şu aşamaları takip edin:
## Projeyi Çalıştırma
1. https://git-scm.com/download/win adresinden kendi sisteminize uygun olan kurulum dosyasını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından aşağıdaki komutlarla git istemcisine kendinizi tanıtın.
```
git config --global user.name "Adınız ve Soyadınız"
git config --global user.email "epostaadresiniz@siteniz.com"
```
2. Komut satırından ```git --version``` komutuyla kurulumu test edin. Karşınızda git’in versiyonunu görüyorsanız git istemcisini başarılı bir şekilde kurdunuz demektir.
3. https://nodejs.org adresinden adresinden kendi sisteminize uygun olan kurulum dosyasını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından ```npm --version``` komutuyla kurulumu test edin. Karşınızda npm’in versiyonunu görüyorsanız kurulum başarılı demektir.
4. https://dotnet.microsoft.com/download/dotnet-core/current/runtime adresinden kendi sisteminize uygun olan ".NET Core Runtime" ve ".NET Core Desktop Runtime" kurulum dosyalarını indirin ve kurulumu başlatın. Kurulum sonrasında komut satırından ```dotnet --version``` komutuyla kurulumu test edin. Karşınızda dotnet’in versiyonunu görüyorsanız kurulum başarılı demektir.
5. Komut satırından ```git clone https://github.com/atifdag/ankacms.git``` komutuyla projeyi bilgisayarınıza indirin.
6. ```npm i -g @angular/cli``` komutuyla "Angular CLI" paketini indirip kurun.
7. Komut satırından projenin olduğu klasörde iken ```npm i``` komutuyla projede kullanılan paketleri kurun.
8. ```ng s -o``` komutuyla projeyi çalıştırın.
