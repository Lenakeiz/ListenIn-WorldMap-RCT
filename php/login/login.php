<?php
require 'conn.php'; 

$id_user = $_POST["id_user"];
$response;

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user WHERE id_user = :id_user LIMIT 1');

$preparedStatement->execute(array('id_user' => $id_user));

if($preparedStatement->rowCount() > 0)
{
	$response = 'exist+';
}
else
{
	$response = 'new_user+';
}

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM app_config WHERE id = 1 LIMIT 1');

$preparedStatement->execute();

if($preparedStatement->rowCount() > 0)
{
	$result = $preparedStatement->fetch();	
	$response .= $result["subscription_needed"];
}
else
{
	$response .= '0';
}

echo $response;
			
?>
