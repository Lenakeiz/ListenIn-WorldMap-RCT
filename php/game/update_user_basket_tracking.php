<?php
require '../game_con/config.php'; 

$id_user = 					$_POST["id_user"];
$Basket1 = 					$_POST["Basket1"];
$Basket2 = 					$_POST["Basket2"];
$Basket3 = 					$_POST["Basket3"];
$Basket4 = 					$_POST["Basket4"];
$Basket5 = 					$_POST["Basket5"];
$Basket6 = 					$_POST["Basket6"];
$Basket7 = 					$_POST["Basket7"];
$Basket8 = 					$_POST["Basket8"];
$DailyTherapyMinutes =      $_POST["DailyTherapyMinutes"];

$preparedStatement = dbConnection::get()->prepare('UPDATE Basket_Tracker SET 
	Basket1 = 					:Basket1,
	Basket2 = 					:Basket2,
	Basket3 = 					:Basket3,
	Basket4 = 					:Basket4,
	Basket5 = 					:Basket5,
	Basket6 = 					:Basket6,
	Basket7 = 					:Basket7,
	Basket8 = 					:Basket8,
	DailyTherapyMinutes =       :DailyTherapyMinutes
WHERE UserID = :id_user LIMIT 1');

$preparedStatement->execute(array(
	'id_user' 			  => $id_user,
	'Basket1'             => $Basket1,
	'Basket2'             => $Basket2,
	'Basket3'             => $Basket3,
	'Basket4'             => $Basket4,
	'Basket5'             => $Basket5,
	'Basket6'             => $Basket6,
	'Basket7'             => $Basket7,
	'Basket8'             => $Basket8,
	'DailyTherapyMinutes' => $DailyTherapyMinutes
));		
?>
