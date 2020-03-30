#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <OneWire.h>
#include <DallasTemperature.h>
//#include <PubSubClient.h>
//#include <PubSubClientTools.h>
#include <Thread.h>             // https://github.com/ivanseidel/ArduinoThread
#include <ThreadController.h>

#ifndef STASSID
#define STASSID "my_wi_fi"
#define STAPSK  "korenimbir9"
#endif

// Data wire is connected to GPIO 2
#define ONE_WIRE_BUS 2

// Setup a oneWire instance to communicate with any OneWire devices
OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature sensor 
DallasTemperature sensors(&oneWire);

const char* ssid = STASSID;
const char* password = STAPSK;

const int led = 13;

//const int mqtt_port = 54893; // Порт для подключения к серверу MQTT
//#define BUFFER_SIZE 100
//#define MQTT_SERVER "fe80::3cd3:39c6:57f:fa73%52"
//const char *mqtt_user = "Login"; // Логи от сервер
//const char *mqtt_pass = "Pass"; // Пароль от сервера

//WiFiClient espClient;
//PubSubClient client(MQTT_SERVER, mqtt_port, espClient);
//PubSubClientTools mqtt(client);


//const String s = "";
ThreadController threadControl = ThreadController();
Thread thread = Thread();

void setup(void) {
  pinMode(led, OUTPUT);
  digitalWrite(led, 0);
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  Serial.println("");

  sensors.begin();

  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  // Connect to MQTT
  // Serial.print(s+"Connecting to MQTT: "+MQTT_SERVER+" ... ");
  // if (client.connect("ESP8266Client")) {
  //   Serial.println("connected");
  //   mqtt.publish("data/device", "{ \"Id\": \"843f16dc-4570-4de4-ac17-376d1a6fdb50\", \"DeviceCode\": \"CustomTemp\", \"Parameter\": 1, \"Type\": 0}");
  // } else {
  //   Serial.println(s+"failed, rc="+client.state());
  // }
  
  //   // Enable Thread
     thread.onRun(publisher);
     thread.setInterval(1000);
     threadControl.add(&thread);
}

void loop(void) {
  //Serial.println("Test serial");
  threadControl.run();
  //delay(500);
    //client.loop();
    
}

void publisher() {
  Serial.println("In publisher serial");
  //mqtt.publish("data/sensors", getTemperature());
}


String getTemperature() {
    // Call sensors.requestTemperatures() to issue a global temperature and Requests to all devices on the bus
  sensors.requestTemperatures(); 
  float tempC = sensors.getTempCByIndex(0);

  if(tempC == -127.00) {
    Serial.println("Failed to read from DS18B20 sensor");
    return "--";
  } else {
    Serial.print("Temperature Celsius: ");
    Serial.println(tempC); 
  }
  return String(tempC);
}