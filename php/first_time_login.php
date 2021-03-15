<?php
require 'conn.php'; 

$user_id = $_POST["user_id"];

$preparedStatement = dbConnection::get()->prepare('UPDATE user SET has_logged = 1 WHERE id = :user_id LIMIT 1');
$preparedStatement->execute(array('user_id' => $user_id));
			
?>
