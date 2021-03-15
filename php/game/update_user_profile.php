<?php
require '../game_con/config.php'; 

$id_user = 					$_POST["id_user"];
$liro_step = 				$_POST["liro_step"];
$is_first_init = 			$_POST["is_first_init"];
$is_tutorial_done = 		$_POST["is_tutorial_done"];
$cycle_number = 			$_POST["cycle_number"];
$therapy_current_block = 	$_POST["therapy_current_block"];
$therapy_total_blocks = 	$_POST["therapy_total_blocks"];
$total_game_time = 			$_POST["total_game_time"];
$total_therapy_time = 		$_POST["total_therapy_time"];
$daily_therapy_time = 		$_POST["daily_therapy_time"];
$act_current_block = 		$_POST["act_current_block"];
$act_total_blocks = 		$_POST["act_total_blocks"];
$practice_completed =		$_POST["practice_completed"];
$test_completed = 			$_POST["test_completed"];
$attempts = 				$_POST["attempts"];
$questionaire_completed = 	$_POST["questionaire_completed"];

$preparedStatement = dbConnection::get()->prepare('UPDATE user_profile SET 
	liro_step = 				:liro_step,
	is_first_init = 			:is_first_init,
	is_tutorial_done = 			:is_tutorial_done,
	cycle_number =				:cycle_number,
	therapy_current_block = 	:therapy_current_block,
	therapy_total_blocks = 		:therapy_total_blocks,
	total_game_time = 			:total_game_time,
	total_therapy_time = 		:total_therapy_time,
	daily_therapy_time = 		:daily_therapy_time,
	act_current_block = 		:act_current_block,
	act_total_blocks = 			:act_total_blocks,
	practice_completed = 		:practice_completed,
	test_completed = 			:test_completed,
	attempts = 					:attempts,
	questionaire_completed = 	:questionaire_completed
WHERE id_user = :id_user LIMIT 1');

$preparedStatement->execute(array(
	'id_user' => $id_user,
	'liro_step' => $liro_step,
	'is_first_init' => $is_first_init,
	'is_tutorial_done' => $is_tutorial_done,
	'cycle_number' => $cycle_number,
	'therapy_current_block' => $therapy_current_block,
	'therapy_total_blocks' => $therapy_total_blocks,
	'total_game_time' => $total_game_time,
	'total_therapy_time' => $total_therapy_time,
	'daily_therapy_time' => $daily_therapy_time,
	'act_current_block' => $act_current_block,
	'act_total_blocks' => $act_total_blocks,
	'practice_completed' => $practice_completed,
	'test_completed' => $test_completed,
	'attempts' => $attempts,
	'questionaire_completed' => $questionaire_completed
));		
?>
