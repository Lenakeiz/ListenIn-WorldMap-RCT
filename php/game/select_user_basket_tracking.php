<?php
require '../game_con/config.php'; 

$id_user = $_POST["id_user"];

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM Basket_Tracker WHERE UserID = :id_user LIMIT 1');
$preparedStatement->execute(array('id_user' => $id_user));

if($preparedStatement->rowCount() > 0)
{
	$result = $preparedStatement->fetch();
	echo $result["Basket1"] . '+' .
	$result["Basket2"] . '+' .
	$result["Basket3"] . '+' .
	$result["Basket4"] . '+' .
	$result["Basket5"] . '+' .
	$result["Basket6"] . '+' .
	$result["Basket7"] . '+' .
	$result["Basket8"] . '+' .
	$result["DailyTherapyMinutes"];
	
}
else
{
	$preparedStatement = dbConnection::get()->prepare('INSERT INTO Basket_Tracker (UserID) VALUES (:id_user)');
	$preparedStatement->execute(array('id_user' => $id_user));
	
	$preparedStatement = dbConnection::get()->prepare('SELECT * FROM Basket_Tracker WHERE UserID = :id_user LIMIT 1');
	$preparedStatement->execute(array('id_user' => $id_user));

	if($preparedStatement->rowCount() > 0)
	{
		$result = $preparedStatement->fetch();
		echo $result["Basket1"] . '+' .
		$result["Basket2"] . '+' .
		$result["Basket3"] . '+' .
		$result["Basket4"] . '+' .
		$result["Basket5"] . '+' .
		$result["Basket6"] . '+' .
		$result["Basket7"] . '+' .
		$result["Basket8"] . '+' .
		$rresut["DailyTherapyMinutes"];
		
	}
}
?>
