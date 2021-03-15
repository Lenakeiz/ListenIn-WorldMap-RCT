<?php
include 'connection.php';

$patient = $_POST["patient"];
$level_start = $_POST["level_start"];
$date = $_POST["date"];
$current_date = new DateTime($date);

//ONLINE WILL GET THE CURRENT DATETIME
//$current_date = new DateTime('now',new DateTimeZone('Europe/London'));

//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));

//if there no therapy, insert directly
if($preparedStatement->rowCount() != 0)
	{
	//GET THE DATE OF THE LAST ONE
	$row = $preparedStatement -> fetch();
	$last_therapy_date = new DateTime($row['date']);

	//DEBUG
	//echo $last_therapy_date->format('Y-m-d H:i:s')."<br>".$current_date->format('Y-m-d H:i:s');
	//echo "<br>";
	//DEBUG

	//getting the hours differnce
	$diff = $current_date->diff($last_therapy_date);
	$hours = $diff->h;
	$hours = $hours + ($diff->days*24);
	//IF THE DIFFERENCE IS BIGGER THAN XX THEN INSERT, IF NOT THEN DON'T INSERT AND USE THAT ONE
	if($hours > 7)
	{
		//INSERT
		$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_daily (id_patient, date, game_level_start) VALUES (:id_patient, :date, :game_level_start)');
		$preparedStatement->execute(array('id_patient' => $patient, 'date' => $current_date->format('Y-m-d H:i:s'), 'game_level_start' => $level_start));
	}
}
else
{
	$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_daily (id_patient, date, game_level_start) VALUES (:id_patient, :date, :game_level_start)');
	$preparedStatement->execute(array('id_patient' => $patient, 'date' => $current_date->format('Y-m-d H:i:s'), 'game_level_start' => $level_start));
}
?>