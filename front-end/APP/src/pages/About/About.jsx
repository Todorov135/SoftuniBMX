import Footer from "../../components/Footer.jsx";
import LoginComponent from "../../components/authComponents/LoginComponent.jsx";
import Navigation from "../../components/Navigation.jsx";
import styles from "./About.module.css";

function About() {
  return (
    <div className={styles.compBody}>
      <Navigation />
      <div className={styles.container}>
        <div className={styles.content}></div>
      </div>
      <Footer />
    </div>
  );
}

export default About;
