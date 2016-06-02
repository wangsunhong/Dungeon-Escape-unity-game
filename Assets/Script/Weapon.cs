using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	public GameCtrl m_gameCtrl;		// 游戏
	public GameObject m_sword;		// 玩家的剑
	public GameObject m_scoreBorad;	// 显示得分的对象
	private AudioChannels m_audio;	// 音频
	public AudioClip m_swordAttackSE;	// 攻击音效
	public GameObject SWORD_ATTACK_OBJ;  // 攻击范围对象
	
	private bool m_equiped = false;  // 装备了宝剑
	private Transform m_target;  // 攻击对象
	
	// 得分
	private const int POINT = 500;
	private const int COMBO_BONUS = 200;
	private int m_combo = 0;
	
	// 初始化
	void Start () {
		m_equiped = false;
		m_sword.renderer.enabled = false;

		// 开始合成动画的结点
		Transform mixTransform = transform.Find("root/hip/mune");

		// 挥动宝剑
		animation["up_sword_action"].layer = 1;
		animation["up_sword_action"].AddMixingTransform(mixTransform);		

		// 把剑举到胸前
		animation["up_sword"].layer = 1;
		animation["up_sword"].AddMixingTransform(mixTransform);			

		m_audio = FindObjectOfType(typeof(AudioChannels)) as AudioChannels;
		m_combo = 0;
	}
	
	// 关卡开始时
	void OnStageStart()
	{
		m_equiped = false;
		m_sword.renderer.enabled = false;
	}
	
	// 拾起宝剑
	void OnGetSword()
	{
		if (!m_equiped) {
			m_sword.renderer.enabled = true;
			m_equiped = true;
			animation.CrossFade("up_sword",0.25f);
		} else {
			BillBoradText borad = ((GameObject)Instantiate(m_scoreBorad,transform.position + new Vector3(0,2.0f,0),Quaternion.identity)).GetComponent<BillBoradText>();
			int point = POINT + COMBO_BONUS * m_combo;
			borad.SetText(point.ToString());
			Score.AddScore(point);
			m_combo++;
		}
	}
	
	void Remove()  
	{
		m_sword.renderer.enabled = false;
		m_equiped = false;
//		animation.CrossFade("up_idle",0.25f);
		animation.Stop("up_sword_action");
		animation.Stop("up_sword");
		m_combo = 0;
	}

	
	// 自动攻击
	public void AutoAttack(Transform other)
	{
		if (m_equiped) {
			m_target = other;
			StartCoroutine("SwordAutoAttack");
		}
	}
	
	// 能够攻击吗？
	public bool CanAutoAttack()
	{
		if (m_equiped)
			return true;
		else
			return false;
	}
		
	
	IEnumerator SwordAutoAttack()
	{
		m_gameCtrl.OnAttack();
		// 回转
		transform.LookAt(m_target.transform);
		yield return null;
		// 攻击
		animation.CrossFade("up_sword_action",0.2f);
		yield return new WaitForSeconds(0.3f);
		m_audio.PlayOneShot(m_swordAttackSE,1.0f,0.0f);		
		yield return new WaitForSeconds(0.2f);
		Vector3 projectilePos;
		projectilePos = transform.position + transform.forward * 0.5f;
		Instantiate(SWORD_ATTACK_OBJ,projectilePos,Quaternion.identity);
		yield return null;
		// 回到原来的方向
		Remove();
		m_gameCtrl.OnEndAttack();
	}
}
