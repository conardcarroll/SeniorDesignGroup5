  <?php
$servername = "localhost";
$username = "root";
$password = "Group5";
$dbname = "Creechky";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
     die("Connection failed: " . $conn->connect_error);
} 

$sql = "SELECT UID, Name, Phone_number, Restriction_type FROM user";
$result = $conn->query($sql);
//$UesrID = $row['UID'];
echo "<form id='frmOne'>";
echo"<table> <tr>
    <th>Add</th>
    <th>Name</th>
	<th>Phone Number</th>
	<th>Image</th>
	<th>Type</th>
  </tr>";

if ($result->num_rows > 0) {
     // output data of each row
         while($row = $result->fetch_assoc()) {
         echo "<tr> <td> <a href='#UpdateInfo' id='{$row['UID']}' onclick='AddUpdateGuest(this.id)'> <input type='radio'  name='all' id='{$row['UID']}'></a> </td><td> ". $row["Name"]. "</td>
		 <td> " . $row["Phone_number"] . "</td> "  . " <td><img src='Eugene.jpg' id='history'></td>
		 <td> " . $row["Restriction_type"] ."</td> </tr>";
     } 
} else {
     echo "0 results";
}
echo "</table></form>";
$conn->close();
?>  