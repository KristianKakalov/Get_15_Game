# Get_15_Game
ООП с С# .NET <br/>
Курсов проект № 11 <br/>

## 1. Проект:<br/>
Проекта за Get 15 представлява имплементация на игра, наподобяваща морски шах, но
разликата се крие в факта, че вместо със символите Х и О се играе с числа. Играе се от
двама души като всеки в своя ход избира число от 1 до 9. Веднъж използвано, едно число
не може да бъде повторено. Целта на всеки играч е да събере три числа, чиято сума е
равна на 15. Победител е този, който пръв успее да събере своите числа, равни на 15.<br/> <br/>
![image](https://user-images.githubusercontent.com/72600322/184536760-03df69a1-4f39-4793-9353-541ab91b5813.png)<br/>

## 2. Архитектура:<br/>
Приложението съдържа сървър, към който се свързват клиентите - играчи. Това ни
позволява лесна комуникация и валидация на ходовете между играчите. При стартиране
на играта, сървърът задължително трябва да бъде активен, в противен случай играта няма
да започне. Към него могат да се свържат само двама на брой играчи, след което играта
стартира като сървъра определя реда и валидацията на всеки ход. На него се записват
всички направени стъпки от страна на играчите и резултата от играта.<br/>
## 3. Структури:<br/>
Основните структури от данни използвани за направата на приложението са HashSet и
List. Те са използвани при написването на логиката на играта (class Game). HashSet се е
с цел запазване на числата, използвани от играчите, и за останалите възможни валидни
числа, които могат да се валидират. В List се съхраняват всички възможни тройки, с
които играчът може да спечели.
## 4. Алгоритми:
Алгоритъм за играта – класа Game представлява масив за запазване на числата на
играча и опонента му. Предварително се задава масив с всички валидни числа, които
могат да се използват, и всички тройки числа, чиято сума е равна на 15. При извършен
ход се подава числото и ID номера на играча (0 или 1), добавя се в съответния масив и се
премахва от масива с възможните числа за игра. След всеки ход сървъра и играчите
проверяват дали играта не е приключила.
Алгоритъм за стартиране на сървъра – при стартиране на сървъра се създава нишка,
която ще очаква връзка от играчите. В отделен клас Player се съдържа информация за
всеки играч и при свързване със сървъра се стартира отделна нишка за всеки
индивидуално. На нея при успешно свързване на играчите се дава начало на играта като
се изпраща съобщение към клиентите, чий ход е.
Алгоритъм за стартиране на клиент – след успешно стартиран сървър, се включват и
клиентите (GameClient), които се свързват към сървъра и се стартира нишка. На нея всеки
клиент получава обратна информация под формата на стринг от сървъра относно
валидността на хода му и чий ред е. Всеки играч има обект Game, в който се запазват
неговите числа, числата на опонента му получени от сървъра и дали играта не е
приключила.
## 5. Тестване и стартиране на проекта:
Играта стартира напълно успешно. След многобройно проведени тестове с различни
невалидни входове и преждевременно изключване на сървъра се установи, че не
възникват проблеми и играта протича гладко.
За да започне играта, първо трябва сървъра да е пуснат в действие, след което
GameClient се стартира два пъти от Start New Instance. Ако всичко е успешно ще се
изшпише кой играч е на ред да играе, играта ще започне и не след дълго ще се установи
кой е победителят.
## 6. Използвана литература:
* Лекциите и упражненията на проф. Кръстев
* Client/Server Tic-Tac-Toe Using a Multithreaded Server в лекция 15
* C# документация от Microsoft
* Различни страници с въпроси (основно stackoverflow.com)
* C# Threads, Tasks, Multi-threading & UI Cross-threading:
https://youtu.be/XXg9g56FS0k
