<?php
require '../game_con/config.php'; 

$id_user = $_POST["id_user"];

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user_profile WHERE id_user = :id_user LIMIT 1');
$preparedStatement->execute(array('id_user' => $id_user));

if($preparedStatement->rowCount() > 0)
{
	$result = $preparedStatement->fetch();
	echo $result["liro_step"] . '+' .
	$result["is_first_init"] . '+' .
	$result["is_tutorial_done"] . '+' .
	$result["cycle_number"] . '+' .
	$result["therapy_current_block"] . '+' .
	$result["therapy_total_blocks"] . '+' .
	$result["total_game_time"] . '+' .
	$result["total_therapy_time"] . '+' .
	$result["daily_therapy_time"] . '+' .
	$result["act_current_block"] . '+' .
	$result["act_total_blocks"] . '+' .
	$result["practice_completed"] . '+' .
	$result["test_completed"] . '+' .
	$result["attempts"] . '+' .
	$result["questionaire_completed"];
	
}
else
{
	$preparedStatement = dbConnection::get()->prepare('INSERT INTO user_profile (id_user) VALUES (:id_user)');
	$preparedStatement->execute(array('id_user' => $id_user));
	
	$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user_profile WHERE id_user = :id_user LIMIT 1');
	$preparedStatement->execute(array('id_user' => $id_user));

	if($preparedStatement->rowCount() > 0)
	{
		$result = $preparedStatement->fetch();
		echo $result["liro_step"] . '+' .
		$result["is_first_init"] . '+' .
		$result["is_tutorial_done"] . '+' .
		$result["cycle_number"] . '+' .
		$result["therapy_current_block"] . '+' .
		$result["therapy_total_blocks"] . '+' .
		$result["total_game_time"] . '+' .
		$result["total_therapy_time"] . '+' .
		$result["daily_therapy_time"] . '+' .
		$result["act_current_block"] . '+' .
		$result["act_total_blocks"] . '+' .
		$result["practice_completed"] . '+' .
		$result["test_completed"] . '+' .
		$result["attempts"] . '+' .
		$result["questionaire_completed"];
		
	}
}
?>
