<?php
include 'connection.php';

$patient_id = $_POST["patientid"];
$date = $_POST["date"];
$total_time = $_POST["totaltime"];

$current_date = new DateTime($date);
$str_current_date = $current_date->format('Y-m-d');

$preparedStatement = $dbConnection->prepare('SELECT * FROM game_time WHERE patient_id = :patient_id ORDER BY date DESC');
$preparedStatement->execute(array('patient_id' => $patient_id));

$bFound = false;
foreach ($preparedStatement as $row) 
{		
	$last_date = new DateTime($row['date']);
	$str_last_date = $last_date->format('Y-m-d');
	
	if ($str_current_date == $str_last_date)
	{
		// update
		$last_id = $row['id'];
		$time = $row['today_game_time_min'] + $total_time;
		
		$preparedStatement = $dbConnection->prepare('UPDATE game_time SET date = :date, today_game_time_min = :today_game_time_min WHERE id = :last_id');	
		
		$preparedStatement->execute(array('last_id' => $last_id, 'date' => $date, 'today_game_time_min' => $time));
		
		$bFound = true;
		break;
	}
}

if ($bFound == false)
{
	// insert the very first entry for this user
	$preparedStatement = $dbConnection->prepare('INSERT INTO game_time (patient_id, date, today_game_time_min) VALUES (:patient_id, :date, :today_game_time_min)');
	
	$preparedStatement->execute(array('patient_id' => $patient_id, 'date' => $date, 'today_game_time_min' => $total_time));	
}

/*
$preparedStatement = $dbConnection->prepare('SELECT * FROM game_time WHERE patient_id = :patient_id and date = (SELECT max(date) FROM game_time WHERE patient_id = :patient_id_date)');
$preparedStatement->execute(array('patient_id' => $patient_id, 'patient_id_date' => $patient_id));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
if ($row)
{
	// update
	$last_date = new DateTime($row['date']);
	$str_last_date = $last_date->format('Y-m-d');
	
	if ($str_current_date == $str_last_date)
	{
		// update
		$last_id = $row['id'];
		$time = $row['today_game_time_min'] + $total_time;
		
		$preparedStatement = $dbConnection->prepare('UPDATE game_time SET date = :date, today_game_time_min = :today_game_time_min WHERE id = :last_id');	
		
		$preparedStatement->execute(array('last_id' => $last_id, 'date' => $date, 'today_game_time_min' => $time));
	}
	else
	{
		// insert new date
		$preparedStatement = $dbConnection->prepare('INSERT INTO game_time (patient_id, date, today_game_time_min) VALUES (:patient_id, :date, :today_game_time_min)');
	
		$preparedStatement->execute(array('patient_id' => $patient_id, 'date' => $date, 'today_game_time_min' => $total_time));
	}
}
else
{
	// insert new
	$preparedStatement = $dbConnection->prepare('INSERT INTO game_time (patient_id, date, today_game_time_min) VALUES (:patient_id, :date, :today_game_time_min)');
	
	$preparedStatement->execute(array('patient_id' => $patient_id, 'date' => $date, 'today_game_time_min' => $total_time));	
}
*/
?>