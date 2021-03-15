<?php
require 'config/config.php'; 

$id_user = $_POST["id_user"];
$code = $_POST["code"];

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM promo_code WHERE code = :code LIMIT 1');
$preparedStatement->execute(array('code' => $code));

if($preparedStatement->rowCount() > 0)
{
	$result = $preparedStatement->fetch();
	
	if($result["id_user"] === NULL)
	{
		$preparedStatementVersion = dbConnection::get()->prepare('UPDATE promo_code SET id_user = :id_user WHERE code = :code LIMIT 1');
		$preparedStatementVersion->execute(array('id_user' => $id_user, 'code' => $code));
		echo "ok";
	}
	else
	{
		echo "error";
	}
}
else
{
	echo "error";
}
?>
