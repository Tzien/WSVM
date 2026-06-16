<template>

<div style="height: calc(100% - 35px); overflow-y: auto;">
    <h2>Cron表达式生成器</h2>
      
    <!-- <el-popover :visible="state.cronPopover" width="600px" > -->
      <vue3CronPlus i18n="cn" @change="changeCron" @close="state.cronPopover = false" max-height="200px" />
      <!-- <template #reference>
        <el-input @click="state.cronPopover = true" v-model="state.cron" placeholder="请选择cron表达式"></el-input>
      </template> -->
    <!-- </el-popover> -->
        
        <div class="alert t-big-margin">
            <!-- <span></span> -->
            <h2>
                cron表达式详解
            </h2>
            <p style="margin: 10px 20px;  padding: 0px;">
                &nbsp;&nbsp;Cron表达式是一个字符串，字符串以<span>5或6</span>个空格隔开，分为<span>6或7</span>个域，每一个域代表一个含义，Cron有如下两种语法格式：
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                &nbsp;&nbsp;&nbsp;&nbsp;（1）&nbsp;Seconds Minutes Hours DayofMonth Month DayofWeek Year
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                &nbsp;&nbsp;&nbsp;&nbsp;（2）<em>Seconds Minutes Hours DayofMonth Month DayofWeek</em>
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <strong>　　</strong>
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <strong>　　<span>一、结构</span></strong>
            </p>
            <p style="margin: 10px 20px;padding: 0px;">
                corn从左到右（用空格隔开）：秒 分 小时 月份中的日期 月份 星期中的日期 年份
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <strong>　　<span>二、各字段的含义</span></strong>
            </p>
            <table class="table" style="margin: 10px 20px; padding: 0px;">
                <tbody>
                    <tr class="firstRow">
                        <td style=" padding: 5px; ">
                            字段
                        </td>
                        <td style=" padding: 5px; ">
                            允许值
                        </td>
                        <td style=" padding: 5px; ">
                            允许的特殊字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            秒（Seconds）
                        </td>
                        <td style=" padding: 5px; ">
                            0~59的整数
                        </td>
                        <td style=" padding: 5px; ">
                            , - * / &nbsp; &nbsp;四个字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            分（<em>Minutes</em>）
                        </td>
                        <td style=" padding: 5px; ">
                            0~59的整数
                        </td>
                        <td style=" padding: 5px; ">
                            , - * / &nbsp; &nbsp;四个字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            小时（<em>Hours</em>）
                        </td>
                        <td style=" padding: 5px; ">
                            0~23的整数
                        </td>
                        <td style=" padding: 5px; ">
                            , - * / &nbsp; &nbsp;四个字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            日期（<em>DayofMonth</em>）
                        </td>
                        <td style=" padding: 5px; ">
                            1~31的整数（但是你需要考虑你月的天数）
                        </td>
                        <td style=" padding: 5px; ">
                            ,- * ? / L W C &nbsp; &nbsp; 八个字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            月份（<em>Month</em>）
                        </td>
                        <td style=" padding: 5px; ">
                            1~12的整数或者 JAN-DEC
                        </td>
                        <td style=" padding: 5px; ">
                            , - * / &nbsp; &nbsp;四个字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            星期（<em>DayofWeek</em>）
                        </td>
                        <td style=" padding: 5px; ">
                            1~7的整数或者 SUN-SAT （1=SUN）
                        </td>
                        <td style=" padding: 5px; ">
                            , - * ? / L C # &nbsp; &nbsp; 八个字符
                        </td>
                    </tr>
                    <tr>
                        <td style=" padding: 5px; ">
                            年(可选，留空)（<em>Year</em>）
                        </td>
                        <td style=" padding: 5px; ">
                            1970~2099
                        </td>
                        <td style=" padding: 5px; ">
                            , - * / &nbsp; &nbsp;四个字符
                        </td>
                    </tr>
                </tbody>
            </table>

            <p style="margin: 10px 20px; padding: 0px;">
                <strong><span>注意事项：</span></strong>
            </p>
            <p style="margin: 10px 20px; padding: 0px;">
                每一个域都使用数字，但还可以出现如下特殊字符，它们的含义是：
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（1）*：表示匹配该域的任意值。</span>假如在Minutes域使用*, 即表示每分钟都会触发事件。
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（2）?：只能用在DayofMonth和DayofWeek两个域。它也匹配域的任意值，但实际不</span>会。因为DayofMonth和DayofWeek会相互影响。例如想在每月的20日触发调度，不管20日到底是星期几，则只能使用如下写法：
                13 13 15 20 * ?, 其中最后一位只能用？，而不能使用*，如果使用*表示不管星期几都会触发，实际上并不是这样。
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（3）-：表示范围。</span>例如在Minutes域使用5-20，表示从5分到20分钟每分钟触发一次&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（4）/：表示起始时间开始触发，然后每隔固定时间触发一次。</span>例如在Minutes域使用5/20,则意味着5分钟触发一次，而25，45等分别触发一次.&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（5）,：表示列出枚举值。</span>例如：在Minutes域使用5,20，则意味着在5和20分每分钟触发一次。&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（6）L：表示最后，只能出现在DayofWeek和DayofMonth域。</span>如果在DayofWeek域使用5L,意味着在最后的一个星期四触发。&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（7）W:表示有效工作日(周一到周五),只能出现在DayofMonth域，系统将在离指定日期的最近的有效工作日触发事件。</span>例如：在
                DayofMonth使用5W，如果5日是星期六，则将在最近的工作日：星期五，即4日触发。如果5日是星期天，则在6日(周一)触发；如果5日在星期一到星期五中的一天，则就在5日触发。另外一点，W的最近寻找不会跨过月份 。
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（8）LW:这两个字符可以连用，表示在某个月最后一个工作日，即最后一个星期五。</span>&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);">（9）#:用于确定每个月第几个星期几，只能出现在DayofWeek域。</span>例如在4#2，表示某月的第二个星期三。
            </p>
            <p style="margin: 10px 20px; padding: 0px;">
                <strong><span>三、常用表达式例子</span></strong>
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span><span style="color: rgb(0, 0, 255);">（0）</span><strong><span style="color: rgb(0, 0, 255);">0/20 * * * * ?</span></strong>&nbsp;&nbsp;</span>&nbsp;表示每20秒
                调整任务
            </p>


            <p style="margin: 10px 20px;  padding: 0px;">
                <span><span style="color: rgb(0, 0, 255);">（1）</span><strong><span style="color: rgb(0, 0, 255);">0 0 2 1 * ?</span></strong>&nbsp;&nbsp;</span>&nbsp;表示在每月的1日的凌晨2点调整任务
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                <span style="color: rgb(0, 0, 255);"><span>（2）</span></span><strong><span style="color: rgb(0, 0, 255);">0 15 10 ? * MON-FRI</span>&nbsp;</strong>&nbsp;
                表示周一到周五每天上午10:15执行作业
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （3）<strong><span style="color: rgb(0, 0, 255);">0 15 10 ? 6L 2002-2006</span></strong>&nbsp;&nbsp;
                表示2002-2006年的每个月的最后一个星期五上午10:15执行作
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （4）<strong><span style="color: rgb(0, 0, 255);">0 0 10,14,16 * * ?</span></strong>&nbsp;&nbsp;&nbsp;每天上午10点，下午2点，4点&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （5）<strong><span style="color: rgb(0, 0, 255);">0 0/30 9-17 * * ?</span></strong>&nbsp;&nbsp;
                朝九晚五工作时间内每半小时&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （6）<strong><span style="color: rgb(0, 0, 255);">0 0 12 ? * WED</span></strong>&nbsp;&nbsp; &nbsp;表示每个星期三中午12点&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （7）<strong><span style="color: rgb(0, 0, 255);">0 0 12 * * ?</span></strong>&nbsp;&nbsp;&nbsp;每天中午12点触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （8）<strong><span style="color: rgb(0, 0, 255);">0 15 10 ? * * &nbsp;</span></strong>&nbsp;&nbsp;每天上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （9）<strong><span style="color: rgb(0, 0, 255);">0 15 10 * * ?</span></strong>&nbsp;&nbsp; &nbsp;
                每天上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （10）<strong><span style="color: rgb(0, 0, 255);">0 15 10 * * ? *</span>&nbsp;</strong>&nbsp; &nbsp;每天上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （11）<strong><span style="color: rgb(0, 0, 255);">0 15 10 * * ? 2005</span></strong>&nbsp;&nbsp; &nbsp;2005年的每天上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （12）<strong><span style="color: rgb(0, 0, 255);">0 * 14 * * ?</span></strong>&nbsp;&nbsp; &nbsp;
                在每天下午2点到下午2:59期间的每1分钟触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （13）<strong><span style="color: rgb(0, 0, 255);">0 0/5 14 * * ?</span></strong>&nbsp;&nbsp; &nbsp;在每天下午2点到下午2:55期间的每5分钟触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （14）<strong><span style="color: rgb(0, 0, 255);">0 0/5 14,18 * * ?</span></strong>&nbsp;&nbsp; &nbsp;
                在每天下午2点到2:55期间和下午6点到6:55期间的每5分钟触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （15）<strong><span style="color: rgb(0, 0, 255);">0 0-5 14 * * ?</span>&nbsp;</strong>&nbsp; &nbsp;在每天下午2点到下午2:05期间的每1分钟触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （16）<strong><span style="color: rgb(0, 0, 255);">0 10,44 14 ? 3 WED</span></strong>&nbsp;&nbsp; &nbsp;每年三月的星期三的下午2:10和2:44触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （17）<strong><span style="color: rgb(0, 0, 255);">0 15 10 ? * MON-FRI</span>&nbsp;</strong>&nbsp; &nbsp;周一至周五的上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （18）<strong><span style="color: rgb(0, 0, 255);">0 15 10 15 * ?</span>&nbsp;</strong>&nbsp; &nbsp;每月15日上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （19）<strong><span style="color: rgb(0, 0, 255);">0 15 10 L * ?</span>&nbsp;</strong>&nbsp; &nbsp;每月最后一日的上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （20）<strong><span style="color: rgb(0, 0, 255);">0 15 10 ? * 6L</span>&nbsp;</strong>&nbsp; &nbsp;每月的最后一个星期五上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （21）<strong><span style="color: rgb(0, 0, 255);">0 15 10 ? * 6L 2002-2005</span></strong>&nbsp;&nbsp;
                2002年至2005年的每月的最后一个星期五上午10:15触发&nbsp;
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （22）<strong><span style="color: rgb(0, 0, 255);">0 15 10 ? * 6#3</span></strong>&nbsp;&nbsp;
                每月的第三个星期五上午10:15触发
            </p>

            <p style="margin: 10px 20px; padding: 0px;">
                <strong><span>　　注：</span></strong>
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                （1）有些子表达式能包含一些范围或列表
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                例如：子表达式（天（星期））可以为 “MON-FRI”，“MON，WED，FRI”，“MON-WED,SAT”<br />“*”字符代表所有可能的值
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                因此，“*”在子表达式（月）里表示每个月的含义，“*”在子表达式（天（星期））表示星期的每一天
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                “/”字符用来指定数值的增量&nbsp;<br>例如：在子表达式（分钟）里的“0/15”表示从第0分钟开始，每15分钟&nbsp;<br>在子表达式（分钟）里的“3/20”表示从第3分钟开始，每20分钟（它和“3，23，43”）的含义一样
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                “？”字符仅被用于天（月）和天（星期）两个子表达式，表示不指定值&nbsp;<br>当2个子表达式其中之一被指定了值以后，为了避免冲突，需要将另一个子表达式的值设为“？”
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                “L” 字符仅被用于天（月）和天（星期）两个子表达式，它是单词“last”的缩写&nbsp;<br>但是它在两个子表达式里的含义是不同的。&nbsp;<br>在天（月）子表达式中，“L”表示一个月的最后一天&nbsp;<br>　　在天（星期）自表达式中，“L”表示一个星期的最后一天，也就是SAT
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                如果在“L”前有具体的内容，它就具有其他的含义了
            </p>
            <p style="margin: 10px 20px;  padding: 0px;">
                例如：“6L”表示这个月的倒数第６天，“FRIL”表示这个月的最一个星期五&nbsp;<br>注意：在使用“L”参数时，不要指定列表或范围，因为这会导致问题
            </p>


        </div>
</div>

</template>


<script setup>

import vue3CronPlus from './vue3-cron-plus/index.vue'
import { reactive } from 'vue'
const state = reactive({
  cronPopover: true,
  cron: 'true'
})
const changeCron = (val) => {
  if (typeof (val) !== 'string') return false
  state.cron = val
}
const togglePopover = (bol) => {
  state.cronPopover = bol
}





</script>


<style  scoped>


</style>