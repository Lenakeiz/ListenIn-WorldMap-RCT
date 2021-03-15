<?php
/*
$id_user = "0";
$file = "thera.csv";
$content = "cacaca";
$header = "caca";*/
//date_default_timezone_set("Europe/London");
//$dateTime = date("d/m/Y h:i:sa");
$checksumPHP = crc32($_POST['content']);
$checksumC = $_POST['checksum'];
if($checksumC == $checksumPHP)
{
	$id_user = $_POST['id_user'];
	$file = $_POST['file_name'];
	$content = $_POST['content'];

	/*if($file == "therapy.csv")
	{
		//date
		$content .= ",".$dateTime."\n";
		$header .= ",".$dateTime."\n";
	}*/
	
	$folder_route = '../files/'.$id_user;
	$file_route = $folder_route.'/'.$file;

	//check for folder, if doesn't exist create
	if (!file_exists($folder_route)) {
		mkdir($folder_route, 0777, true);
	}

	//Use the function is_file to check if the file already exists or not.
	if(!is_file($file_route))
	{
		//Save our content to the file.
		file_put_contents($file_route, $content);
	}
	else
	{
		file_put_contents($file_route, $content, FILE_APPEND | LOCK_EX);
	}

	$file_content = file_get_contents($file_route);
	echo $file_content;	
}
else
{
	echo "checksum_error";
}


?>
