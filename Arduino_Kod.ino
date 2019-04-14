
char x;
int LedK=6;
int LedY=8;
int LedM=7;
void setup() {

    Serial.begin(9600);
    pinMode(LedK,OUTPUT);
    pinMode(LedY,OUTPUT);
    pinMode(LedM,OUTPUT);  
}

void loop() {
  while(Serial.available()>0)
  {
    
    Serial.flush();
    x=Serial.read();
    
    switch(x)
    {
      case 'K':  
      digitalWrite(LedK,HIGH);
      delay(100);
      digitalWrite(LedK,LOW);
     
      break;

      case 'Y':
      digitalWrite(LedY,HIGH);
      delay(100);
      digitalWrite(LedY,LOW);
      break;

      case 'B':
      digitalWrite(LedM,HIGH);
      delay(100);
      digitalWrite(LedM,LOW);
      break;
      
    }
}
}




