#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include <PubSubClient.h>
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

ThreadController threadControl = ThreadController();
Thread thread = Thread();

const char* mqtt_server = "192.168.1.9";
WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;


void setup(void) {
  pinMode(led, OUTPUT);
  digitalWrite(led, 0);
  Serial.begin(115200);
  
  setupWifi();

  sensors.begin();

  setupThread();

  client.setServer(mqtt_server, 54893);
}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266ClientCustomTempSensor";
    // Attempt to connect
    if (client.connect(clientId.c_str())) {
      Serial.println("connected");
      client.publish("data/device", "{ \"Id\": \"843f16dc-4570-4de4-ac17-376d1a6fdb50\", \"DeviceCode\": \"CustomTemp\", \"Parameter\": 1, \"Type\": 0}");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}


void setupWifi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  
  Serial.println("");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}

void setupThread() {
 // Enable Thread
  thread.onRun(publisher);
  thread.setInterval(20000);
  threadControl.add(&thread);
}

void loop(void) {
  threadControl.run();
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
}

void publisher() {
  String sensorValue = "{ \"RawValue\": \"" + getTemperature() + "\", \"DeviceId\": \"843f16dc-4570-4de4-ac17-376d1a6fdb50\"}";
  client.publish("data/sensors", sensorValue.c_str());
}


String getTemperature() {
    // Call sensors.requestTemperatures() to issue a global temperature and Requests to all devices on the bus
  sensors.requestTemperatures(); 
  float tempC = sensors.getTempCByIndex(0);

  if(tempC == -127.00) {
    Serial.println("Failed to read from DS18B20 sensor");
    return "-9999";
  } else {
    Serial.print("Temperature Celsius: ");
    Serial.println(tempC); 
  }
  return String(tempC);
}