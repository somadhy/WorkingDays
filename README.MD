# WorkingDays

Is simple API for fork with buiseness days

## Using 

### Configure
Add  weekdays and weekends that do not match the calendar to the file ```.\data\dates.txt```
File format is date;type one date per row where type is 

```cs
0 - Unknown
1 - Off day
2 - Working day
```
### Run

1. With dotnet Kestrel
   
```bash
dotnet run 
```

2. Publish as exeutable Kestrel
   
```bash
dotnet publish -c Release --output ./dist

.\dist\WorkingDays.exe
```

3. Docker Kestrel
   
```bash
docker compose up # --build
```
If you dont want to mount the data folder, just comment or delete section ```volumes``` in ```docker-compose.yml```


### Query API

Swagger: `http://localhost:{port}/index.html`

Currently implemented:

Get all dates from repository:

```bash
curl -X 'GET' \
  'http://localhost:5024/days' \
  -H 'accept: application/json'
```

Response body

```json
[
  {
    "day": "2024-01-01T00:00:00",
    "dayType": 1,
    "dayTypeAsString": "Выходной день"
  },
  ...
]
```

Checking the date 01.01.2024 status: 

```bash
curl -X 'GET' \
  'http://localhost:5024/days/20240101' \
  -H 'accept: application/json'

```
Response body

```json
{
  "day": "2024-01-01T00:00:00",
  "dayType": 1,
  "dayTypeAsString": "Выходной день"
}
```

Adding 5 business days to a date 31.12.2023:

``` bash
curl -X 'GET' \
  'http://localhost:5024/days/20231231/add/days/5' \
  -H 'accept: application/json'
```

Response body

```json
{
  "day": "2024-01-05T00:00:00",
  "dayType": 1,
  "dayTypeAsString": "Выходной день"
}
```

Adding 10 any days to the date 31.12.2023:

```bash
curl -X 'GET' \
  'http://localhost:5024/days/20231231/add/workingDays/10' \
  -H 'accept: application/json'
```

Response body

```json
{
  "day": "2024-01-22T00:00:00",
  "dayType": 2,
  "dayTypeAsString": "Рабочий день"
}
```

## Dependencies
- Serilog
- Swashbuckle Swagger